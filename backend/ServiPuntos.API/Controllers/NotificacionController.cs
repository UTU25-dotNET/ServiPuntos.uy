using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;
using System.Security.Claims;

namespace ServiPuntos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NotificacionController : ControllerBase
    {
        private readonly INotificacionService _service;
        private readonly ITenantContext _tenantContext;
        private readonly IUsuarioRepository _usuarioRepository;

        public NotificacionController(INotificacionService service, ITenantContext tenantContext, IUsuarioRepository usuarioRepository)
        {
            _service = service;
            _tenantContext = tenantContext;
            _usuarioRepository = usuarioRepository;
        }

        private async Task<Guid> GetUserIdAsync()
        {
            // Buscar el identificador de usuario en diferentes claims para
            // asegurar compatibilidad con distintos flujos de autenticación
            var claim =
                User.FindFirst(ClaimTypes.NameIdentifier) ??
                User.FindFirst("nameid");

            if (claim != null && Guid.TryParse(claim.Value, out var guid))
            {
                return guid;
            }

            // Fallback: intentar obtenerlo por email si solo tenemos 'sub' u otro identificador
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

        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var userId = await GetUserIdAsync();
            if (userId == Guid.Empty) return Unauthorized();
            var list = await _service.ObtenerPorUsuarioAsync(userId);
            var result = list.Select(n => new
            {
                id = n.Id,
                titulo = n.Notificacion?.Titulo,
                mensaje = n.Notificacion?.Mensaje,
                fecha = n.Notificacion?.FechaCreacion,
                leida = n.Leida
            });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CreateNotificacionRequest request)
        {
            var tenantId = _tenantContext.TenantId;
            if (tenantId == Guid.Empty) return Unauthorized();
            var notif = new Notificacion
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                AudienciaId = request.AudienciaId,
                Titulo = request.Titulo,
                Mensaje = request.Mensaje,
                FechaCreacion = DateTime.UtcNow
            };
            await _service.CrearNotificacionAsync(notif, request.AudienciaId);
            return Ok(new { id = notif.Id });
        }

        [HttpPut("leida/{id}")]
        public async Task<IActionResult> MarcarLeida(Guid id)
        {
            await _service.MarcarComoLeidaAsync(id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(Guid id)
        {
            await _service.EliminarParaUsuarioAsync(id);
            return NoContent();
        }

        [HttpDelete("mine")]
        public async Task<IActionResult> EliminarTodas()
        {
            var userId = await GetUserIdAsync();
            if (userId == Guid.Empty) return Unauthorized();
            await _service.DeleteAllByUsuarioAsync(userId);
            return NoContent();
        }
    }

    public class CreateNotificacionRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public Guid? AudienciaId { get; set; }
    }
}