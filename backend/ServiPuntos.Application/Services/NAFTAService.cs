using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Core.NAFTA;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServiPuntos.Application.Services
{
    public class NAFTAService : INAFTAService
    {
        private readonly ITransaccionService _transaccionService;
        private readonly ICanjeService _canjeService;
        private readonly IPuntosService _puntosService;
        private readonly IUsuarioService _usuarioService;
        private readonly ITenantService _tenantService;
        private readonly IUbicacionService _ubicacionService;

        public NAFTAService(
            ITransaccionService transaccionService,
            ICanjeService canjeService,
            IPuntosService puntosService,
            IUsuarioService usuarioService,
            ITenantService tenantService,
            IUbicacionService ubicacionService)
        {
            _transaccionService = transaccionService;
            _canjeService = canjeService;
            _puntosService = puntosService;
            _usuarioService = usuarioService;
            _tenantService = tenantService;
            _ubicacionService = ubicacionService;
        }

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
                Guid tenantId = mensaje.TenantId;
                Guid ubicacionId = mensaje.UbicacionId;

                // Verificar que tenant y ubicación existan
                var tenant = await _tenantService.GetByIdAsync(tenantId);
                var ubicacion = await _ubicacionService.GetUbicacionAsync(ubicacionId);

                if (tenant == null || ubicacion == null)
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, "Tenant o ubicación no encontrados");
                }

                // Deserializar la transacción del mensaje
                TransaccionNAFTA transaccion;
                try
                {
                    transaccion = JsonSerializer.Deserialize<TransaccionNAFTA>(
                        JsonSerializer.Serialize(mensaje.Datos["transaccion"]));
                }
                catch (Exception ex)
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, $"Error al deserializar la transacción: {ex.Message}");
                }

                // Procesar la transacción
                RespuestaPuntosNAFTA respuestaPuntos = await _transaccionService.ProcesarTransaccionNAFTAAsync(
                    transaccion, tenantId, ubicacionId);

                // Crear respuesta exitosa
                var respuesta = new RespuestaNAFTA
                {
                    IdMensajeReferencia = mensaje.IdMensaje,
                    Codigo = "OK",
                    Mensaje = "Transacción procesada correctamente",
                    Datos = new System.Collections.Generic.Dictionary<string, object>
                    {
                        { "respuestaPuntos", respuestaPuntos }
                    }
                };

                return respuesta;
            }
            catch (Exception ex)
            {
                return CrearErrorRespuesta(mensaje.IdMensaje, $"Error al procesar la transacción: {ex.Message}");
            }
        }

        public async Task<RespuestaNAFTA> VerificarUsuarioAsync(MensajeNAFTA mensaje)
        {
            try
            {
                // Validar el mensaje
                if (mensaje == null || !mensaje.Datos.ContainsKey("identificadorUsuario"))
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, "Formato de mensaje inválido");
                }

                string identificadorUsuario = mensaje.Datos["identificadorUsuario"].ToString();

                // Buscar usuario
                var usuario = await _usuarioService.GetUsuarioAsync(identificadorUsuario);
                if (usuario == null)
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, "Usuario no encontrado");
                }

                // Obtener saldo de puntos
                Guid tenantId = mensaje.TenantId;
                int saldoPuntos = await _puntosService.GetSaldoByUsuarioIdAsync(usuario.Id);

                // Crear respuesta exitosa
                var respuesta = new RespuestaNAFTA
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

                return respuesta;
            }
            catch (Exception ex)
            {
                return CrearErrorRespuesta(mensaje.IdMensaje, $"Error al verificar usuario: {ex.Message}");
            }
        }

        public async Task<RespuestaNAFTA> ProcesarCanjeAsync(MensajeNAFTA mensaje)
        {
            try
            {
                // Validar el mensaje
                if (mensaje == null || !mensaje.Datos.ContainsKey("canje"))
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, "Formato de mensaje inválido");
                }

                // Deserializar el canje del mensaje
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

                // Verificar que el código QR exista y sea válido
                var canjeExistente = await _canjeService.GetCanjeByCodigoQRAsync(canje.CodigoQR);
                if (canjeExistente == null)
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, "Código QR no encontrado");
                }

                // Procesar el canje
                bool resultado = await _canjeService.ProcesarCanjeAsync(canje);

                if (!resultado)
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, "Error al procesar el canje");
                }

                // Crear respuesta exitosa
                var respuesta = new RespuestaNAFTA
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

                return respuesta;
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
                // Validar el mensaje
                if (mensaje == null || !mensaje.Datos.ContainsKey("identificadorUsuario"))
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, "Formato de mensaje inválido");
                }

                string identificadorUsuario = mensaje.Datos["identificadorUsuario"].ToString();
                Guid tenantId = mensaje.TenantId;

                // Buscar usuario
                var usuario = await _usuarioService.GetUsuarioAsync(identificadorUsuario);
                if (usuario == null)
                {
                    return CrearErrorRespuesta(mensaje.IdMensaje, "Usuario no encontrado");
                }

                // Obtener saldo de puntos
                int saldoPuntos = await _puntosService.GetSaldoByUsuarioIdAsync(usuario.Id);

                // Crear respuesta exitosa
                var respuesta = new RespuestaNAFTA
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

                return respuesta;
            }
            catch (Exception ex)
            {
                return CrearErrorRespuesta(mensaje.IdMensaje, $"Error al consultar saldo: {ex.Message}");
            }
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
    }
}