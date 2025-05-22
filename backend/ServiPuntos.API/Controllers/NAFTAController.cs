using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Core.NAFTA;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiPuntos.API.Controllers
{
    [ApiController]
    [Route("api/nafta")]
    public class NAFTAController : ControllerBase
    {
        private readonly INAFTAService _naftaService;

        public NAFTAController(INAFTAService naftaService)
        {
            _naftaService = naftaService;
        }

        /// <summary>
        /// Procesa una transacción enviada por el sistema POS
        /// </summary>
        [HttpPost("transaccion")]
        [AllowAnonymous]
        public async Task<ActionResult<RespuestaNAFTA>> ProcesarTransaccion([FromBody] MensajeNAFTA mensaje)
        {
            if (mensaje == null)
                return BadRequest("El mensaje no puede ser nulo");

            var respuesta = await _naftaService.ProcesarTransaccionAsync(mensaje);
            if (respuesta.Codigo == "ERROR")
                return BadRequest(respuesta);

            return Ok(respuesta);
        }

        /// <summary>
        /// Verifica la existencia y validez de un usuario
        /// </summary>
        [HttpPost("verificar-usuario")]
        [AllowAnonymous]
        public async Task<ActionResult<RespuestaNAFTA>> VerificarUsuario([FromBody] MensajeNAFTA mensaje)
        {
            if (mensaje == null)
                return BadRequest("El mensaje no puede ser nulo");

            var respuesta = await _naftaService.VerificarUsuarioAsync(mensaje);
            if (respuesta.Codigo == "ERROR")
                return BadRequest(respuesta);

            return Ok(respuesta);
        }

        /// <summary>
        /// Procesa un canje de puntos mediante código QR
        /// </summary>
        [HttpPost("canje")]
        [AllowAnonymous]
        public async Task<ActionResult<RespuestaNAFTA>> ProcesarCanje([FromBody] MensajeNAFTA mensaje)
        {
            if (mensaje == null)
                return BadRequest("El mensaje no puede ser nulo");

            var respuesta = await _naftaService.ProcesarCanjeAsync(mensaje);
            if (respuesta.Codigo == "ERROR")
                return BadRequest(respuesta);

            return Ok(respuesta);
        }

        /// <summary>
        /// Consulta el saldo de puntos de un usuario
        /// </summary>
        [HttpPost("consultar-saldo")]
        [AllowAnonymous]
        public async Task<ActionResult<RespuestaNAFTA>> ConsultarSaldo([FromBody] MensajeNAFTA mensaje)
        {
            if (mensaje == null)
                return BadRequest("El mensaje no puede ser nulo");

            var respuesta = await _naftaService.ConsultarSaldoAsync(mensaje);
            if (respuesta.Codigo == "ERROR")
                return BadRequest(respuesta);

            return Ok(respuesta);
        }

        /// <summary>
        /// Endpoint de prueba para verificar que el servicio NAFTA está operativo
        /// </summary>
        [HttpGet("ping")]
        [AllowAnonymous]
        public ActionResult<string> Ping() => Ok("Servicio NAFTA operativo");

        /// <summary>
        /// Retorna la especificación del protocolo NAFTA
        /// </summary>
        [HttpGet("especificacion")]
        [AllowAnonymous]
        public ActionResult<object> GetEspecificacion()
        {
            var espec = new
            {
                nombre = "NAFTA - Negocio Avanzado de Fidelización en Terminales de Autoservicio",
                version = "1.0",
                formatoMensajes = "JSON",
                endpoints = new[]
                {
                    new { ruta = "/api/nafta/transaccion",       metodo = "POST", descripcion = "Registra una transacción y asigna puntos" },
                    new { ruta = "/api/nafta/verificar-usuario", metodo = "POST", descripcion = "Verifica la existencia de un usuario" },
                    new { ruta = "/api/nafta/canje",            metodo = "POST", descripcion = "Procesa el canje de un código QR" },
                    new { ruta = "/api/nafta/consultar-saldo",  metodo = "POST", descripcion = "Consulta el saldo de puntos de un usuario" }
                },
                tiposMensajes = new[]
                {
                    new { tipo = "MensajeNAFTA",           descripcion = "Estructura base para todas las comunicaciones" },
                    new { tipo = "TransaccionNAFTA",       descripcion = "Datos de transacción (compra, minimercado, etc.)" },
                    new { tipo = "CanjeNAFTA",             descripcion = "Datos para procesar un canje de puntos" },
                    new { tipo = "RespuestaNAFTA",         descripcion = "Estructura de respuesta estándar" },
                    new { tipo = "RespuestaPuntosNAFTA",   descripcion = "Detalles específicos de puntos en respuesta" }
                },
                tiposTransacciones = new[]
                {
                    new { codigo = "combustible",  descripcion = "Compra de combustible" },
                    new { codigo = "minimercado",  descripcion = "Compra en minimercado" },
                    new { codigo = "servicio",     descripcion = "Uso de servicios (lavadero, etc.)" }
                },
                ejemplos = new
                {
                    transaccion = new
                    {
                        peticion = new MensajeNAFTA
                        {
                            Version = "1.0",
                            IdMensaje = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                            TipoMensaje = "transaccion",
                            UbicacionId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                            TenantId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                            TerminalId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                            Datos = new Dictionary<string, object>
                            {
                                ["transaccion"] = new
                                {
                                    IdTransaccion = "tx123456",
                                    IdentificadorUsuario = "user@example.com",
                                    TipoTransaccion = "combustible",
                                    Monto = 1500.00,
                                    MetodoPago = "efectivo",
                                    Productos = new[]
                                    {
                                        new
                                        {
                                            IdProducto     = "nafta-super",
                                            NombreProducto = "Nafta Super",
                                            Categoria       = "combustible",
                                            Cantidad        = 15.00,
                                            PrecioUnitario  = 100.00,
                                            SubTotal        = 1500.00
                                        }
                                    }
                                }
                            }
                        },
                        respuesta = new RespuestaNAFTA
                        {
                            IdMensajeReferencia = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                            Codigo = "OK",
                            Mensaje = "Transacción procesada correctamente",
                            Datos = new Dictionary<string, object>
                            {
                                ["respuestaPuntos"] = new
                                {
                                    IdentificadorUsuario = "user@example.com",
                                    PuntosOtorgados = 15.00,
                                    SaldoActual = 115.00,
                                    SaldoAnterior = 100.00
                                }
                            }
                        }
                    }
                }
            };

            return Ok(espec);
        }
    }
}
