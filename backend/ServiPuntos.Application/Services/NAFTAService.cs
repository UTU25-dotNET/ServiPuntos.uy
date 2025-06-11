using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Core.NAFTA;
using ServiPuntos.Core.DTOs;
using ServiPuntos.Core.Enums;
using System;
using System.Text.Json;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiPuntos.Application.Services
{
    public class NAFTAService : INAFTAService
    {
        private readonly ICanjeService _canjeService;
        private readonly IPuntosService _puntosService;
        private readonly IUsuarioService _usuarioService;
        private readonly ITenantService _tenantService;
        private readonly IUbicacionService _ubicacionService;
        private readonly IPayPalService _payPalService;
        private readonly ITransaccionRepository _transaccionRepository;

        private readonly IProductoUbicacionService _productoUbicacionService;

        public NAFTAService(
            ICanjeService canjeService,
            IPuntosService puntosService,
            IUsuarioService usuarioService,
            ITenantService tenantService,
            IUbicacionService ubicacionService,
            IPayPalService payPalService,
            ITransaccionRepository transaccionRepository,
            IProductoUbicacionService productoUbicacionService)
        {
            _canjeService = canjeService;
            _puntosService = puntosService;
            _usuarioService = usuarioService;
            _tenantService = tenantService;
            _ubicacionService = ubicacionService;
            _payPalService = payPalService;
            _transaccionRepository = transaccionRepository;
            _productoUbicacionService = productoUbicacionService;
        }

        /// <summary>
        /// Procesa transacciones que involucran PayPal (puro o mixto con puntos como descuento)
        /// </summary>
        public async Task<RespuestaNAFTA> ProcesarTransaccionAsync(MensajeNAFTA mensaje)
        {
            try
            {
                // Validar el mensaje
                if (mensaje == null || !mensaje.Datos.ContainsKey("transaccion"))
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, "Formato de mensaje inválido");
                }

                // Obtener tenant y ubicación
                var tenant = await _tenantService.GetByIdAsync(mensaje.TenantId);
                var ubicacion = await _ubicacionService.GetUbicacionAsync(mensaje.UbicacionId);

                if (tenant == null || ubicacion == null)
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, "Tenant o ubicación no encontrados");
                }

                // Deserializar la transacción del mensaje
                TransaccionNAFTA transaccionNAFTA;
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    transaccionNAFTA = JsonSerializer.Deserialize<TransaccionNAFTA>(
                        JsonSerializer.Serialize(mensaje.Datos["transaccion"]),
                        options);
                }
                catch (Exception ex)
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, $"Error al deserializar la transacción: {ex.Message}");
                }

                // Validar que es una transacción válida (debe tener monto PayPal)
                if (transaccionNAFTA.MontoPayPal <= 0)
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, "Las transacciones deben incluir monto PayPal. Para pagos solo con puntos, usar endpoint de canje.");
                }

                // Verificar usuario
                var usuario = await _usuarioService.GetUsuarioAsync(transaccionNAFTA.IdentificadorUsuario);
                if (usuario == null)
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, "Usuario no encontrado");
                }

                // Si es transacción mixta, verificar saldo de puntos
                if (transaccionNAFTA.EsTransaccionMixta)
                {
                    var saldoPuntos = await _puntosService.GetSaldoByUsuarioIdAsync(usuario.Id);
                    if (saldoPuntos < transaccionNAFTA.PuntosUtilizados)
                    {
                        return CrearErrorRespuesta(mensaje.IdMensaje, "Saldo de puntos insuficiente para el descuento");
                    }
                }

                // Si no tiene PaymentId, crear el pago en PayPal
                if (string.IsNullOrEmpty(transaccionNAFTA.PayPalPaymentId))
                {
                    var payPalPayment = await _payPalService.CreatePaymentAsync(
                        transaccionNAFTA.MontoPayPal,
                        "USD",
                        $"Compra en {ubicacion.Nombre}");

                    // Crear transacción pendiente en BD
                    var transaccionPendiente = new Transaccion
                    {
                        Id = transaccionNAFTA.IdTransaccion,
                        UsuarioId = transaccionNAFTA.IdentificadorUsuario,
                        UbicacionId = mensaje.UbicacionId,
                        TenantId = mensaje.TenantId,
                        FechaTransaccion = transaccionNAFTA.FechaTransaccion,
                        TipoTransaccion = transaccionNAFTA.TipoTransaccion,
                        Monto = transaccionNAFTA.Monto,
                        MontoPayPal = transaccionNAFTA.MontoPayPal,
                        PuntosUtilizados = transaccionNAFTA.PuntosUtilizados,
                        PagoPayPalId = payPalPayment.PaymentId,
                        EstadoPayPal = "CREATED",
                        PuntosOtorgados = 0, // Se calculan después del pago
                        Detalles = JsonSerializer.Serialize(new
                        {
                            Productos = transaccionNAFTA.Productos,
                            DatosAdicionales = transaccionNAFTA.DatosAdicionales
                        })
                    };

                    await _transaccionRepository.AddAsync(transaccionPendiente);

                    payPalPayment.AdditionalData.TryGetValue("approval_url", out var urlObj);
                    var approvalUrl = urlObj?.ToString();

                    return new RespuestaNAFTA
                    {
                        IdMensajeReferencia = mensaje.IdMensaje,
                        Codigo = "PENDING_PAYMENT",
                        Mensaje = "Pago PayPal creado, esperando aprobación del usuario",
                        Datos = new System.Collections.Generic.Dictionary<string, object>
                        {
                            { "paymentId", payPalPayment.PaymentId },
                            { "approvalUrl", approvalUrl },
                            { "transaccionId", transaccionNAFTA.IdTransaccion },
                            { "montoPayPal", transaccionNAFTA.MontoPayPal },
                            { "puntosUtilizados", transaccionNAFTA.PuntosUtilizados }
                        }
                    };
                }

                // Si tiene PaymentId, verificar y completar la transacción
                return await CompletarTransaccion(transaccionNAFTA, mensaje);
            }
            catch (Exception ex)
            {
                return CrearErrorRespuesta(mensaje.IdMensaje, $"Error al procesar la transacción: {ex.Message}");
            }
        }

        /// <summary>
        /// Confirma un pago de PayPal después de la aprobación del usuario
        /// </summary>
        public async Task<RespuestaNAFTA> ConfirmarPagoPayPalAsync(MensajeNAFTA mensaje)
        {
            try
            {
                if (mensaje == null || !mensaje.Datos.ContainsKey("paymentId") || !mensaje.Datos.ContainsKey("payerId"))
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, "Datos de confirmación PayPal incompletos");
                }

                string paymentId = mensaje.Datos["paymentId"].ToString();
                string payerId = mensaje.Datos["payerId"].ToString();

                // Ejecutar el pago en PayPal
                var paymentResult = await _payPalService.ExecutePaymentAsync(paymentId, payerId);

                // Actualizar la transacción local
                var transaccion = await _transaccionRepository.GetByPayPalPaymentIdAsync(paymentId);
                if (transaccion == null)
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, "Transacción no encontrada");
                }

                transaccion.PayPalPayerId = payerId;
                transaccion.EstadoPayPal = paymentResult.Status;
                transaccion.FechaCompletadoPayPal = DateTime.UtcNow;

                if (paymentResult.Status == "approved")
                {
                    // Si es transacción mixta, descontar puntos
                    if (transaccion.PuntosUtilizados > 0)
                    {
                        await _puntosService.DebitarPuntosAsync(transaccion.UsuarioId, transaccion.PuntosUtilizados);
                    }

                    // Calcular y otorgar puntos por la compra (basado en el monto total, no solo PayPal)
                    var puntosGanados = await CalcularPuntosGanadosAsync(transaccion.Monto, transaccion.TipoTransaccion, transaccion.TenantId);
                    transaccion.PuntosOtorgados = puntosGanados;

                    if (puntosGanados > 0)
                    {
                        await _puntosService.ActualizarSaldoAsync(transaccion.UsuarioId, puntosGanados);
                    }

                    await ActualizarStockAsync(transaccion.UbicacionId, transaccion.Detalles);
                }



                await _transaccionRepository.UpdateAsync(transaccion);

                var saldoActual = await _puntosService.GetSaldoByUsuarioIdAsync(transaccion.UsuarioId);

                return new RespuestaNAFTA
                {
                    IdMensajeReferencia = mensaje.IdMensaje,
                    Codigo = paymentResult.Status == "approved" ? "OK" : "ERROR",
                    Mensaje = paymentResult.Status == "approved" ? "Transacción completada exitosamente" : "Error en el pago",
                    Datos = new System.Collections.Generic.Dictionary<string, object>
                    {
                        { "paymentStatus", paymentResult.Status },
                        { "transaccionId", transaccion.Id },
                        { "puntosUtilizados", transaccion.PuntosUtilizados },
                        { "puntosGanados", transaccion.PuntosOtorgados },
                        { "saldoActual", saldoActual }
                    }
                };
            }
            catch (Exception ex)
            {
                return CrearErrorRespuesta(mensaje.IdMensaje, $"Error al confirmar pago PayPal: {ex.Message}");
            }
        }

        private async Task<RespuestaNAFTA> CompletarTransaccion(TransaccionNAFTA transaccionNAFTA, MensajeNAFTA mensaje)
        {
            // Verificar el pago de PayPal
            var isValid = await _payPalService.ValidatePaymentAsync(
                transaccionNAFTA.PayPalPaymentId,
                transaccionNAFTA.MontoPayPal);

            if (!isValid)
            {
                return CrearErrorRespuesta(mensaje.IdMensaje, "Pago de PayPal no válido o no completado");
            }

            // Buscar la transacción existente
            var transaccion = await _transaccionRepository.GetByPayPalPaymentIdAsync(transaccionNAFTA.PayPalPaymentId);
            if (transaccion == null)
            {
                return CrearErrorRespuesta(mensaje.IdMensaje, "Transacción no encontrada");
            }

            // Actualizar con datos de confirmación
            transaccion.PayPalPayerId = transaccionNAFTA.PayPalPayerId;
            transaccion.PayPalToken = transaccionNAFTA.PayPalToken;
            transaccion.EstadoPayPal = "APPROVED";
            transaccion.FechaCompletadoPayPal = DateTime.UtcNow;

            // Procesar puntos
            if (transaccion.PuntosUtilizados > 0)
            {
                await _puntosService.DebitarPuntosAsync(transaccion.UsuarioId, transaccion.PuntosUtilizados);
            }

            var puntosGanados = await CalcularPuntosGanadosAsync(transaccion.Monto, transaccion.TipoTransaccion, transaccion.TenantId);
            transaccion.PuntosOtorgados = puntosGanados;

            if (puntosGanados > 0)
            {
                await _puntosService.ActualizarSaldoAsync(transaccion.UsuarioId, puntosGanados);
            }
            await ActualizarStockAsync(transaccion.UbicacionId, transaccion.Detalles);
            await _transaccionRepository.UpdateAsync(transaccion);

            var saldoFinal = await _puntosService.GetSaldoByUsuarioIdAsync(transaccion.UsuarioId);

            return new RespuestaNAFTA
            {
                IdMensajeReferencia = mensaje.IdMensaje,
                Codigo = "OK",
                Mensaje = "Transacción procesada correctamente",
                Datos = new System.Collections.Generic.Dictionary<string, object>
                {
                    { "transaccionId", transaccion.Id },
                    { "puntosUtilizados", transaccion.PuntosUtilizados },
                    { "puntosGanados", transaccion.PuntosOtorgados },
                    { "saldoActual", saldoFinal }
                }
            };
        }

        public async Task<RespuestaNAFTA> VerificarUsuarioAsync(MensajeNAFTA mensaje)
        {
            try
            {
                if (mensaje == null || !mensaje.Datos.ContainsKey("identificadorUsuario"))
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, "Formato de mensaje inválido");
                }

                string identificadorUsuario = mensaje.Datos["identificadorUsuario"].ToString();
                var usuario = await _usuarioService.GetUsuarioAsync(identificadorUsuario);

                if (usuario == null)
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, "Usuario no encontrado");
                }

                int saldoPuntos = await _puntosService.GetSaldoByUsuarioIdAsync(usuario.Id);

                return new RespuestaNAFTA
                {
                    IdMensajeReferencia = mensaje.IdMensaje,
                    Codigo = "OK",
                    Mensaje = "Usuario verificado",
                    Datos = new System.Collections.Generic.Dictionary<string, object>
                    {
                        { "usuarioId", usuario.Id },
                        { "nombre", usuario.Nombre },
                        { "apellido", usuario.Apellido },
                        { "saldoPuntos", saldoPuntos }
                    }
                };
            }
            catch (Exception ex)
            {
                return CrearErrorRespuesta(mensaje.IdMensaje, $"Error al verificar usuario: {ex.Message}");
            }
        }

        public async Task<RespuestaNAFTA> GenerarCanjeAsync(MensajeNAFTA mensaje)
        {
            try
            {
                if (mensaje == null && !mensaje.Datos.ContainsKey("productoCanjeableId")! && mensaje.Datos.ContainsKey("usuarioId"))
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, "Formato de mensaje inválido");
                }

                if (!Guid.TryParse(mensaje.Datos["productoCanjeableId"].ToString(), out Guid productoId) ||
                    !Guid.TryParse(mensaje.Datos["usuarioId"].ToString(), out Guid usuarioId))
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, "Datos de canje inválidos");
                }

                var codigo = await _canjeService.GenerarCodigoCanjeAsync(usuarioId, productoId, mensaje.UbicacionId, mensaje.TenantId);

                return new RespuestaNAFTA
                {
                    IdMensajeReferencia = mensaje.IdMensaje,
                    Codigo = "OK",
                    Mensaje = "Canje generado correctamente",
                    Datos = new System.Collections.Generic.Dictionary<string, object>
                    {
                        { "codigoQR", codigo }
                    }
                };
            }
            catch (Exception ex)
            {
                return CrearErrorRespuesta(mensaje.IdMensaje, $"Error al generar el canje: {ex.Message}");
            }
        }

        public async Task<RespuestaNAFTA> GenerarCanjesAsync(MensajeNAFTA mensaje)
        {
            try
            {
                if (mensaje == null || !mensaje.Datos.ContainsKey("productoIds") || !mensaje.Datos.ContainsKey("usuarioId"))
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, "Formato de mensaje inválido");
                }

                List<Guid> productos;
                try
                {
                    productos = JsonSerializer.Deserialize<List<Guid>>(JsonSerializer.Serialize(mensaje.Datos["productoIds"])) ?? new List<Guid>();
                }
                catch (Exception ex)
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, $"Error al deserializar productos: {ex.Message}");
                }

                if (!Guid.TryParse(mensaje.Datos["usuarioId"].ToString(), out Guid usuarioId))
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, "UsuarioId inválido");
                }

                var resultados = new List<object>();
                foreach (var prodId in productos)
                {
                    try
                    {
                        var codigo = await _canjeService.GenerarCodigoCanjeAsync(usuarioId, prodId, mensaje.UbicacionId, mensaje.TenantId);
                        resultados.Add(new { productoId = prodId, codigoQR = codigo });
                    }
                    catch (Exception ex)
                    {
                        resultados.Add(new { productoId = prodId, error = ex.Message });
                    }
                }

                return new RespuestaNAFTA
                {
                    IdMensajeReferencia = mensaje.IdMensaje,
                    Codigo = "OK",
                    Mensaje = "Canjes generados",
                    Datos = new Dictionary<string, object>
                    {
                        { "resultados", resultados }
                    }
                };
            }
            catch (Exception ex)
            {
                return CrearErrorRespuesta(mensaje.IdMensaje, $"Error al generar canjes: {ex.Message}");
            }
        }

        /// <summary>
        /// Procesa canjes de productos usando solo puntos (genera QR)
        /// </summary>
        public async Task<RespuestaNAFTA> ProcesarCanjeAsync(MensajeNAFTA mensaje)
        {
            try
            {
                if (mensaje == null || !mensaje.Datos.ContainsKey("canje"))
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, "Formato de mensaje inválido");
                }

                CanjeNAFTA canje;
                try
                {
                    canje = JsonSerializer.Deserialize<CanjeNAFTA>(
                        JsonSerializer.Serialize(mensaje.Datos["canje"]));
                }
                catch (Exception ex)
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, $"Error al deserializar el canje: {ex.Message}");
                }

                var canjeExistente = await _canjeService.GetCanjeByCodigoQRAsync(canje.CodigoQR);
                if (canjeExistente == null)
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, "Código QR de canje no encontrado");
                }

                bool resultado = await _canjeService.ProcesarCanjeAsync(canje);

                if (!resultado)
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, "Error al procesar el canje");
                }

                return new RespuestaNAFTA
                {
                    IdMensajeReferencia = mensaje.IdMensaje,
                    Codigo = "OK",
                    Mensaje = "Canje procesado correctamente",
                    Datos = new System.Collections.Generic.Dictionary<string, object>
                    {
                        { "canjeId", canjeExistente.Id },
                        { "productoCanjeable", canjeExistente.ProductoCanjeableId },
                        { "puntosCanjeados", canjeExistente.PuntosCanjeados }
                    }
                };
            }
            catch (Exception ex)
            {
                return CrearErrorRespuesta(mensaje.IdMensaje, $"Error al procesar el canje: {ex.Message}");
            }
        }

        public async Task<RespuestaNAFTA> ConsultarSaldoAsync(MensajeNAFTA mensaje)
        {
            try
            {
                if (mensaje == null || !mensaje.Datos.ContainsKey("identificadorUsuario"))
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, "Formato de mensaje inválido");
                }

                string identificadorUsuarioStr = mensaje.Datos["identificadorUsuario"].ToString();

                if (!Guid.TryParse(identificadorUsuarioStr, out Guid identificadorUsuario))
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, "Identificador de usuario no válido");
                }

                var usuario = await _usuarioService.GetUsuarioAsync(identificadorUsuario);

                if (usuario == null)
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, "Usuario no encontrado");
                }

                int saldoPuntos = await _puntosService.GetSaldoByUsuarioIdAsync(usuario.Id);

                return new RespuestaNAFTA
                {
                    IdMensajeReferencia = mensaje.IdMensaje,
                    Codigo = "OK",
                    Mensaje = "Consulta de saldo exitosa",
                    Datos = new System.Collections.Generic.Dictionary<string, object>
                    {
                        { "usuarioId", usuario.Id },
                        { "saldoPuntos", saldoPuntos }
                    }
                };
            }
            catch (Exception ex)
            {
                return CrearErrorRespuesta(mensaje.IdMensaje, $"Error al consultar saldo: {ex.Message}");
            }
        }


        private async Task<int> CalcularPuntosGanadosAsync(decimal monto, TipoTransaccion tipo, Guid tenantId)
        {
            var tenant = await _tenantService.GetByIdAsync(tenantId);
            if (tenant == null)
            {
                return 0;
            }

            decimal tasa = tipo switch
            {
                TipoTransaccion.CompraCombustible => tenant.TasaCombustible,
                TipoTransaccion.CompraMinimercado => tenant.TasaMinimercado,
                TipoTransaccion.UsoServicio => tenant.TasaServicios,
                _ => 0m
            };

            return (int)Math.Round(monto * tasa);
        }

        private RespuestaNAFTA CrearErrorRespuesta(Guid idMensajeReferencia, string mensajeError)
        {
            return new RespuestaNAFTA
            {
                IdMensajeReferencia = idMensajeReferencia,
                Codigo = "ERROR",
                Mensaje = mensajeError
            };
        }
        
        private async Task ActualizarStockAsync(Guid ubicacionId, string detallesJson)
        {
            if (string.IsNullOrWhiteSpace(detallesJson))
                return;

            try
            {
                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                List<LineaTransaccionNAFTA>? productos = null;

                if (detallesJson.TrimStart().StartsWith("["))
                {
                    productos = JsonSerializer.Deserialize<List<LineaTransaccionNAFTA>>(detallesJson, opts);
                }
                else
                {
                    var detalles = JsonSerializer.Deserialize<DetallesTransaccion>(detallesJson, opts);
                    productos = detalles?.Productos;
                }

                if (productos == null || productos.Count == 0)
                    return;

                var productosUbicacion = await _productoUbicacionService.GetAllAsync(ubicacionId);
                foreach (var linea in productos)
                {
                    var prod = productosUbicacion.FirstOrDefault(p => p.ProductoCanjeableId == linea.IdProducto);
                    if (prod != null)
                    {
                        prod.StockDisponible = Math.Max(0, prod.StockDisponible - linea.Cantidad);
                        await _productoUbicacionService.UpdateAsync(prod);
                    }
                }
            }
            catch
            {
                // Ignorar errores de actualización de stock
            }
        }

        private class DetallesTransaccion
        {
            public List<LineaTransaccionNAFTA> Productos { get; set; } = new();
        }
    }
}