using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using ServiPuntos.Core.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ServiPuntos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CanjeController : ControllerBase
    {
        private readonly ICanjeService _canjeService;

        public CanjeController(ICanjeService canjeService)
        {
            _canjeService = canjeService;
        }

        // GET api/canje/usuario/{usuarioId}
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> GetCanjesByUsuario(Guid usuarioId)
        {
            try
            {
                var canjes = await _canjeService.GetCanjesByUsuarioIdAsync(usuarioId);

                var response = canjes.Select(c => new
                {
                    id = c.Id,
                    producto = c.ProductoCanjeable?.Nombre,
                    ubicacion = c.Ubicacion?.Nombre,
                    fechaGeneracion = c.FechaGeneracion,
                    fechaCanje = c.FechaCanje,
                    estado = c.Estado.ToString(),
                    puntos = c.PuntosCanjeados
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los canjes", error = ex.Message });
            }
        }

        // GET api/canje/ubicacion/{ubicacionId}/pendientes
        [HttpGet("ubicacion/{ubicacionId}/pendientes")]
        public async Task<IActionResult> GetPendientesByUbicacion(Guid ubicacionId)
        {
            var canjes = await _canjeService.GetCanjesPendientesByUbicacionIdAsync(ubicacionId);

            var response = canjes.Select(c => new
            {
                id = c.Id,
                producto = c.ProductoCanjeable?.Nombre,
                usuario = c.Usuario?.Nombre,
                fechaGeneracion = c.FechaGeneracion,
                fechaExpiracion = c.FechaExpiracion,
                puntos = c.PuntosCanjeados
            });

            return Ok(response);
        }

        // POST api/canje/{canjeId}/confirmar
        [HttpPost("{canjeId}/confirmar")]
        public async Task<IActionResult> ConfirmarCanje(Guid canjeId)
        {
            try
            {
                await _canjeService.ConfirmarCanjeAsync(canjeId);
                return Ok(new { message = "Canje confirmado" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}