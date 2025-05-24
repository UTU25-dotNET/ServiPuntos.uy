using Microsoft.AspNetCore.Mvc;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Core.Entities;

namespace ServiPuntos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UbicacionController : ControllerBase
    {
        private readonly IUbicacionService _ubicacionService;

        public UbicacionController(IUbicacionService ubicacionService)
        {
            _ubicacionService = ubicacionService;
        }

        /// <summary>
        /// Obtener todas las ubicaciones de un tenant específico
        /// </summary>
        /// <param name="tenantId">ID del tenant</param>
        /// <returns>Lista de ubicaciones del tenant</returns>
        [HttpGet("tenant/{tenantId}")]
        public async Task<IActionResult> GetUbicacionesByTenant(Guid tenantId)
        {
            try
            {
                Console.WriteLine($"[GetUbicacionesByTenant] Obteniendo ubicaciones para tenant: {tenantId}");
                
                var ubicaciones = await _ubicacionService.GetAllUbicacionesAsync(tenantId);
                
                Console.WriteLine($"[GetUbicacionesByTenant] Se encontraron {ubicaciones.Count()} ubicaciones");
                
                return Ok(ubicaciones);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetUbicacionesByTenant] Error: {ex.Message}");
                return StatusCode(500, new { message = "Error al obtener las ubicaciones" });
            }
        }

        /// <summary>
        /// Obtener todas las ubicaciones con sus precios de combustible
        /// </summary>
        /// <returns>Lista de ubicaciones con precios</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllUbicaciones()
        {
            try
            {
                Console.WriteLine("[GetAllUbicaciones] Obteniendo todas las ubicaciones");
                
                var ubicaciones = await _ubicacionService.GetAllUbicacionesAsync();
                
                Console.WriteLine($"[GetAllUbicaciones] Se encontraron {ubicaciones.Count()} ubicaciones");
                
                return Ok(ubicaciones);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetAllUbicaciones] Error: {ex.Message}");
                return StatusCode(500, new { message = "Error al obtener las ubicaciones" });
            }
        }

        /// <summary>
        /// Obtener una ubicación específica por ID
        /// </summary>
        /// <param name="id">ID de la ubicación</param>
        /// <returns>Datos de la ubicación</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUbicacion(Guid id)
        {
            try
            {
                var ubicacion = await _ubicacionService.GetUbicacionAsync(id);
                
                if (ubicacion == null)
                {
                    return NotFound(new { message = "Ubicación no encontrada" });
                }
                
                return Ok(ubicacion);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetUbicacion] Error: {ex.Message}");
                return StatusCode(500, new { message = "Error al obtener la ubicación" });
            }
        }
    }
}