using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using ServiPuntos.Core.Interfaces;
using System.Security.Claims;

namespace ServiPuntos.API.Controllers
{
    [ApiController]
    [Route("api/dispositivos")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DispositivoController : ControllerBase
    {
        private readonly IDispositivoService _service;
        private readonly IUsuarioRepository _usuarioRepository;

        public DispositivoController(IDispositivoService service, IUsuarioRepository usuarioRepository)
        {
            _service = service;
            _usuarioRepository = usuarioRepository;
        }

        private async Task<Guid> GetUserIdAsync()
        {
            var claim =
                User.FindFirst(ClaimTypes.NameIdentifier) ??
                User.FindFirst("nameid");

            if (claim != null && Guid.TryParse(claim.Value, out var guid))
            {
                return guid;
            }

            var emailClaim = User.FindFirst(ClaimTypes.Email) ?? User.FindFirst("email");
            if (emailClaim != null)
            {
                var user = await _usuarioRepository.GetByEmailAsync(emailClaim.Value);
                if (user != null)
                {
                    return user.Id;
                }
            }

            var subClaim = User.FindFirst("sub");
            if (subClaim != null && Guid.TryParse(subClaim.Value, out guid))
            {
                return guid;
            }

            return Guid.Empty;
        }

        [HttpPost("token")]
        public async Task<IActionResult> RegistrarToken([FromBody] RegistrarTokenRequest request)
        {
            var userId = await GetUserIdAsync();
            if (userId == Guid.Empty) return Unauthorized();
            await _service.RegistrarTokenAsync(userId, request.Token);
            return NoContent();
        }
    }

    public class RegistrarTokenRequest
    {
        public string Token { get; set; } = string.Empty;
    }
}
