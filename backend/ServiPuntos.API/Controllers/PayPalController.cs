using Microsoft.AspNetCore.Mvc;

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
            return Ok(new
            {
                message = "Pago aprobado exitosamente",
                paymentId = paymentId,
                payerId = PayerID,
                token = token,
                instructions = "Usar estos datos en /api/nafta/confirmar-paypal"
            });
        }

        [HttpGet("cancel")]
        public IActionResult PayPalCancel([FromQuery] string token)
        {
            // Esta URL se llama cuando el usuario CANCELA el pago en PayPal
            return Ok(new
            {
                message = "Pago cancelado por el usuario",
                token = token
            });
        }
    }
}
