using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiPuntos.Core.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ServiPuntos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
    }
}
