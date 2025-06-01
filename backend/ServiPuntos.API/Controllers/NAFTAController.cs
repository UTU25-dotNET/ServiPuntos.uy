using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Core.NAFTA;
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
        [AllowAnonymous] // Las terminales POS pueden necesitar acceso sin autenticación
        public async Task<ActionResult<RespuestaNAFTA>> ProcesarTransaccion([FromBody] MensajeNAFTA mensaje)
        {
            if (mensaje == null)
            {
                return BadRequest("El mensaje no puede ser nulo");
            }

            var respuesta = await _naftaService.ProcesarTransaccionAsync(mensaje);

            if (respuesta.Codigo == "ERROR")
            {
                return BadRequest(respuesta);
            }

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
            {
                return BadRequest("El mensaje no puede ser nulo");
            }

            var respuesta = await _naftaService.VerificarUsuarioAsync(mensaje);

            if (respuesta.Codigo == "ERROR")
            {
                return BadRequest(respuesta);
            }

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
            {
                return BadRequest("El mensaje no puede ser nulo");
            }

            var respuesta = await _naftaService.ProcesarCanjeAsync(mensaje);

            if (respuesta.Codigo == "ERROR")
            {
                return BadRequest(respuesta);
            }

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
            {
                return BadRequest("El mensaje no puede ser nulo");
            }

            var respuesta = await _naftaService.ConsultarSaldoAsync(mensaje);

            if (respuesta.Codigo == "ERROR")
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        /// <summary>
        /// Endpoint de prueba para verificar que el servicio NAFTA está operativo
        /// </summary>
        [HttpGet("ping")]
        [AllowAnonymous]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]  // Requiere token JWT válido
        public ActionResult<string> Ping()
        {
            return Ok("Servicio NAFTA operativo");
        }


    }
}