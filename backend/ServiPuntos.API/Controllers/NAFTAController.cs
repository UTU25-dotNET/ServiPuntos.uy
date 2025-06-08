using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiPuntos.Application.Services;
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

        // Procesa una transacción enviada por el sistema 
        [HttpPost("transaccion")]
        [AllowAnonymous] // Las terminales POS pueden necesitar acceso sin autenticación
        public async Task<ActionResult<RespuestaNAFTA>> ProcesarTransaccion([FromBody] MensajeNAFTA mensaje)
        {
            if (mensaje == null)
            {
                return BadRequest("El mensaje no puede ser nulo");
            }

            var respuesta = await _naftaService.ProcesarTransaccionAsync(mensaje);

            // Para transacciones pendientes de pago PayPal, devolver 202 Accepted
            if (respuesta.Codigo == "PENDING_PAYMENT")
            {
                return Accepted(respuesta);
            }

            if (respuesta.Codigo == "ERROR")
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        /// <summary>
        /// Confirma un pago de PayPal después de la aprobación del usuario
        /// </summary>
        [HttpPost("confirmar-paypal")]
        [AllowAnonymous]
        public async Task<ActionResult<RespuestaNAFTA>> ConfirmarPagoPayPal([FromBody] MensajeNAFTA mensaje)
        {
            if (mensaje == null)
            {
                return BadRequest("El mensaje no puede ser nulo");
            }

            var respuesta = await _naftaService.ConfirmarPagoPayPalAsync(mensaje);

            if (respuesta.Codigo == "ERROR")
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // Verifica la existencia y validez de un usuario
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

        // Genera un canje para que el usuario obtenga el código QR
        [HttpPost("generar-canje")]
        [AllowAnonymous]
        public async Task<ActionResult<RespuestaNAFTA>> GenerarCanje([FromBody] MensajeNAFTA mensaje)
        {
            if (mensaje == null)
            {
                return BadRequest("El mensaje no puede ser nulo");
            }

            var respuesta = await _naftaService.GenerarCanjeAsync(mensaje);

            if (respuesta.Codigo == "ERROR")
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        [HttpPost("generar-canjes")]
        [AllowAnonymous]
        public async Task<ActionResult<RespuestaNAFTA>> GenerarCanjes([FromBody] MensajeNAFTA mensaje)
        {
            if (mensaje == null)
            {
                return BadRequest("El mensaje no puede ser nulo");
            }

            var respuesta = await _naftaService.GenerarCanjesAsync(mensaje);

            if (respuesta.Codigo == "ERROR")
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // Procesa un canje de puntos mediante código QR
        [HttpPost("procesar-canje")]
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

        // Consulta el saldo de puntos de un usuario
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

        // Endpoint de prueba para verificar que el servicio NAFTA está operativo
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("ping")]
        public ActionResult<string> Ping()
        {
            // Debugging temporal
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            Console.WriteLine($"Authorization header: {authHeader ?? "NO ENCONTRADO"}");
    
            var user = User?.Identity?.Name;
            Console.WriteLine($"Usuario autenticado: {user ?? "NO AUTENTICADO"}");
    
            return Ok("Servicio NAFTA operativo");
        }
    }
}