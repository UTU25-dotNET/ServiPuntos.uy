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
            var html = $@"<!DOCTYPE html>
            <html lang=""es"">
            <head>
                <meta charset=\""UTF-8\"">
                <meta name=\""viewport\"" content=\""width=device-width, initial-scale=1.0\"">
                <title>Compra completada</title>
                <link rel=\""stylesheet\"" href=\""https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css\"">
            </head>
            <body class=\""container py-5\"">
                <h1 class=\""mb-4\"">¡Pago aprobado!</h1>
                <p><strong>Payment ID:</strong> {paymentId}</p>
                <p><strong>Payer ID:</strong> {PayerID}</p>
                <p><strong>Token:</strong> {token}</p>
                <a href=\""/\"" class=\""btn btn-primary mt-3\"">Volver al inicio</a>
            </body>
            </html>";

            return new ContentResult
            {
                Content = html,
                ContentType = "text/html"
            };
        }

        [HttpGet("cancel")]
        public IActionResult PayPalCancel([FromQuery] string token)
        {
            // Esta URL se llama cuando el usuario CANCELA el pago en PayPal
            var html = $@"<!DOCTYPE html>
            <html lang=\""es\"">
            <head>
                <meta charset=\""UTF-8\"">
                <meta name=\""viewport\"" content=\""width=device-width, initial-scale=1.0\"">
                <title>Pago cancelado</title>
                <link rel=\""stylesheet\"" href=\""https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css\"">
            </head>
            <body class=\""container py-5\"">
                <h1 class=\""mb-4\"">Pago cancelado por el usuario</h1>
                <p><strong>Token:</strong> {token}</p>
                <a href=\""/\"" class=\""btn btn-primary mt-3\"">Volver al inicio</a>
            </body>
            </html>";

            return new ContentResult
            {
                Content = html,
                ContentType = "text/html"
            };
        }
    }
}