using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
=======
using System;
>>>>>>> origin/dev

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
<<<<<<< HEAD
            return Ok(new
            {
                message = "Pago aprobado exitosamente",
                paymentId = paymentId,
                payerId = PayerID,
                token = token,
                instructions = "Usar estos datos en /api/nafta/confirmar-paypal"
            });
=======
            var redirectUrl = $"http://localhost:3000/paypal-return?paymentId={Uri.EscapeDataString(paymentId)}&payerId={Uri.EscapeDataString(PayerID)}&token={Uri.EscapeDataString(token)}";
            return Redirect(redirectUrl);
>>>>>>> origin/dev
        }

        [HttpGet("cancel")]
        public IActionResult PayPalCancel([FromQuery] string token)
        {
            // Esta URL se llama cuando el usuario CANCELA el pago en PayPal
<<<<<<< HEAD
            return Ok(new
            {
                message = "Pago cancelado por el usuario",
                token = token
            });
        }
    }
}
=======
            var redirectUrl = $"http://localhost:3000/paypal-cancel?token={Uri.EscapeDataString(token)}";
            return Redirect(redirectUrl);
        }
    }
}
>>>>>>> origin/dev
