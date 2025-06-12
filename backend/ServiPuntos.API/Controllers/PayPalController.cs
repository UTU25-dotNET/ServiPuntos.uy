using Microsoft.AspNetCore.Mvc;
using System;

namespace ServiPuntos.API.Controllers
{
    [ApiController]
    [Route("api/paypal")]
    public class PayPalController : ControllerBase
    {
        [HttpGet("return")]
        public IActionResult PayPalReturn([FromQuery] string paymentId, [FromQuery] string PayerID, [FromQuery] string token)
        {
            // Esta URL se llama cuando el usuario APRUEBA el pago en PayPal
            var redirectUrl = $"http://localhost:3000/paypal-return?paymentId={Uri.EscapeDataString(paymentId)}&payerId={Uri.EscapeDataString(PayerID)}&token={Uri.EscapeDataString(token)}";
            return Redirect(redirectUrl);
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