using Microsoft.AspNetCore.Mvc;

namespace ServiPuntos.API.Controllers
{
    [ApiController]
    [Route("api/paypal")]
    public class PayPalController : ControllerBase
    {
    private readonly IConfiguration _config;
    public PayPalController(IConfiguration config) => _config = config;

    [HttpGet("return")]
    public IActionResult PayPalReturn([FromQuery] string paymentId, [FromQuery] string PayerID, [FromQuery] string token)
    {
        var frontendUrl = _config["FRONTEND_URL"] ?? "http://localhost:3000";
        var url = $"{frontendUrl.TrimEnd('/')}/paypal/confirm?paymentId={paymentId}&PayerID={PayerID}&token={token}";
        return Redirect(url);
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
