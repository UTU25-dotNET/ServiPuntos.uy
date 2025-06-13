using Microsoft.AspNetCore.Mvc;
using System;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Core.NAFTA;

namespace ServiPuntos.API.Controllers
{
    [ApiController]
    [Route("api/paypal")]
    public class PayPalController : ControllerBase
    {
        private readonly INAFTAService _naftaService;

        public PayPalController(INAFTAService naftaService)
        {
            _naftaService = naftaService;
        }

        [HttpGet("return")]

        public async Task<IActionResult> PayPalReturn([FromQuery] string paymentId, [FromQuery] string PayerID, [FromQuery] string token)
        {
            // Cuando el usuario aprueba el pago en PayPal, confirmamos la transacción
            var mensaje = new MensajeNAFTA
        {
            // Esta URL se llama cuando el usuario APRUEBA el pago en PayPal
            TipoMensaje = Core.Enums.TipoMensajeNAFTA.Transaccion,
                Datos = new System.Collections.Generic.Dictionary<string, object>
                {
                    { "paymentId", paymentId },
                    { "payerId", PayerID },
                    { "token", token }
                }
            };

            var respuesta = await _naftaService.ConfirmarPagoPayPalAsync(mensaje);
            return Ok(respuesta);
        }

        [HttpGet("cancel")]
        public IActionResult PayPalCancel([FromQuery] string token)
        {
            // Esta URL se llama cuando el usuario CANCELA el pago en PayPal
            var redirectUrl = $"http://localhost:3000/paypal-cancel?token={Uri.EscapeDataString(token)}";
            return Redirect(redirectUrl);
        }
    }
}