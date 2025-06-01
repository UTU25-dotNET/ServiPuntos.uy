using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc; // Necesario para [ApiController], [Route], IActionResult, etc.
using Microsoft.Extensions.Logging;
using ServiPuntos.Core.Interfaces; // Donde está IAudienciaService y los DTOs
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.DTOs;  // Para la entidad Audiencia si la devuelves directamente

// Si usas un DTO para la salida de GetDefinicionesAudienciaAsync, defínelo.
// Por ejemplo:
// public class AudienciaDefinicionDto
// {
//     public Guid Id { get; set; }
//     public string NombreUnicoInterno { get; set; }
//     public string NombreDescriptivo { get; set; }
//     public int Prioridad { get; set; }
//     public bool Activa { get; set; }
//     public int NumeroDeReglas { get; set; }
// }


[ApiController]
[Route("api/tenants/{tenantId}/audiencias")] // Ruta base para las audiencias de un tenant
// [Authorize] // Asegúrate de proteger tus endpoints
public class AudienciasController : ControllerBase
{
    private readonly IAudienciaService _audienciaService;
    private readonly ILogger<AudienciasController> _logger;
    private readonly ITenantContext _tenantContext; // Opcional, si obtienes tenantId del contexto

    public AudienciasController(
        IAudienciaService audienciaService,
        ILogger<AudienciasController> logger,
        ITenantContext tenantContext = null) // Hacerlo opcional si siempre viene en la ruta
    {
        _audienciaService = audienciaService ?? throw new ArgumentNullException(nameof(audienciaService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _tenantContext = tenantContext; // Puede ser null si no se inyecta globalmente
    }

    /// <summary>
    /// Obtiene el TenantId a usar, priorizando el de la ruta, luego el del contexto.
    /// </summary>
    private Guid GetEffectiveTenantId(Guid tenantIdFromRoute)
    {
        if (tenantIdFromRoute != Guid.Empty) return tenantIdFromRoute;
        if (_tenantContext != null && _tenantContext.TenantId != Guid.Empty) return _tenantContext.TenantId;

        _logger.LogError("TenantId no pudo ser determinado.");
        throw new InvalidOperationException("TenantId es requerido y no pudo ser determinado.");
    }

    /// <summary>
    /// Crea una nueva definición de audiencia o actualiza una existente si se provee un ID en el DTO.
    /// </summary>
    /// <param name="tenantId">ID del tenant.</param>
    /// <param name="dto">Datos de la audiencia a crear o actualizar.</param>
    /// <returns>La audiencia creada o actualizada.</returns>
    [HttpPost] // Usar POST para crear, PUT para actualizar completamente, PATCH para actualizar parcialmente
    [ProducesResponseType(typeof(Audiencia), 201)] // 201 Created
    [ProducesResponseType(typeof(Audiencia), 200)] // 200 OK (si actualiza)
    [ProducesResponseType(400)] // Bad Request
    [ProducesResponseType(404)] // Not Found (si se intenta actualizar una audiencia inexistente por ID)
    public async Task<IActionResult> GuardarAudiencia(Guid tenantId, [FromBody] AudienciaDto dto)
    {
        tenantId = GetEffectiveTenantId(tenantId);
        if (dto == null) return BadRequest("El cuerpo de la solicitud no puede ser nulo.");
        if (!ModelState.IsValid) return BadRequest(ModelState); // Validar DTO con DataAnnotations

        try
        {
            _logger.LogInformation("API: Solicitud para guardar audiencia '{NombreUnico}' para TenantId {TenantId}", dto.NombreUnicoInterno, tenantId);
            Audiencia audienciaGuardada = await _audienciaService.GuardarAudienciaAsync(tenantId, dto);

            if (dto.Id == Guid.Empty || audienciaGuardada.FechaCreacion == audienciaGuardada.FechaModificacion) // Asumir que si las fechas son iguales, es nueva
            {
                _logger.LogInformation("API: Audiencia '{NombreUnico}' creada con ID {AudienciaId}", audienciaGuardada.NombreUnicoInterno, audienciaGuardada.Id);
                // Para POST (crear), es común devolver 201 Created con la URI del nuevo recurso.
                return CreatedAtAction(nameof(GetAudienciaPorId), new { tenantId = tenantId, audienciaId = audienciaGuardada.Id }, audienciaGuardada);
            }
            else
            {
                _logger.LogInformation("API: Audiencia '{NombreUnico}' actualizada con ID {AudienciaId}", audienciaGuardada.NombreUnicoInterno, audienciaGuardada.Id);
                return Ok(audienciaGuardada); // Para PUT/PATCH (actualizar), 200 OK o 204 No Content.
            }
        }
        catch (InvalidOperationException ex) // Ej: NombreUnicoInterno duplicado
        {
            _logger.LogWarning(ex, "API: Error de operación al guardar audiencia para TenantId {TenantId}: {ErrorMessage}", tenantId, ex.Message);
            return Conflict(new { message = ex.Message }); // 409 Conflict
        }
        catch (KeyNotFoundException ex) // Ej: Audiencia no encontrada para actualizar
        {
            _logger.LogWarning(ex, "API: Audiencia no encontrada al intentar guardar para TenantId {TenantId}: {ErrorMessage}", tenantId, ex.Message);
            return NotFound(new { message = ex.Message }); // 404 Not Found
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "API: Argumento inválido al guardar audiencia para TenantId {TenantId}: {ErrorMessage}", tenantId, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API: Error inesperado al guardar audiencia para TenantId {TenantId}", tenantId);
            return StatusCode(500, "Ocurrió un error interno al procesar la solicitud.");
        }
    }

    /// <summary>
    /// Obtiene la definición de una audiencia específica por su ID.
    /// </summary>
    [HttpGet("{audienciaId}")]
    [ProducesResponseType(typeof(Audiencia), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetAudienciaPorId(Guid tenantId, Guid audienciaId)
    {
        tenantId = GetEffectiveTenantId(tenantId);
        _logger.LogInformation("API: Solicitud para obtener audiencia por ID {AudienciaId} para TenantId {TenantId}", audienciaId, tenantId);

        // IAudienciaService debería tener un método para esto, o usamos el repositorio directamente si es solo lectura de definición.
        // Por ahora, asumimos que GetDefinicionesAudienciaAsync y filtramos. Mejor si hay un GetById específico.
        var audiencia = (await _audienciaService.GetDefinicionesAudienciaAsync(tenantId))
                        .FirstOrDefault(a => a.Id == audienciaId);

        if (audiencia == null)
        {
            _logger.LogWarning("API: Audiencia por ID {AudienciaId} no encontrada para TenantId {TenantId}", audienciaId, tenantId);
            return NotFound();
        }
        return Ok(audiencia);
    }


    /// <summary>
    /// Obtiene todas las definiciones de audiencia para un tenant.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Audiencia>), 200)] // Podrías usar un AudienciaDefinicionDto aquí
    public async Task<IActionResult> GetTodasLasDefinicionesDeAudiencia(Guid tenantId)
    {
        tenantId = GetEffectiveTenantId(tenantId);
        _logger.LogInformation("API: Solicitud para obtener todas las definiciones de audiencia para TenantId {TenantId}", tenantId);
        var definiciones = await _audienciaService.GetDefinicionesAudienciaAsync(tenantId);
        // Si usas DTOs para la salida:
        // var dtos = definiciones.Select(a => new AudienciaDefinicionDto { ... mapeo ... });
        // return Ok(dtos);
        return Ok(definiciones);
    }

    /// <summary>
    /// Elimina una definición de audiencia.
    /// </summary>
    [HttpDelete("{audienciaId}")]
    [ProducesResponseType(204)] // No Content
    [ProducesResponseType(404)]
    public async Task<IActionResult> EliminarAudiencia(Guid tenantId, Guid audienciaId)
    {
        tenantId = GetEffectiveTenantId(tenantId);
        _logger.LogInformation("API: Solicitud para eliminar audiencia ID {AudienciaId} para TenantId {TenantId}", audienciaId, tenantId);
        try
        {
            await _audienciaService.EliminarAudienciaAsync(tenantId, audienciaId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "API: Audiencia ID {AudienciaId} no encontrada para eliminar en TenantId {TenantId}", audienciaId, tenantId);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API: Error inesperado al eliminar audiencia ID {AudienciaId} para TenantId {TenantId}", audienciaId, tenantId);
            return StatusCode(500, "Ocurrió un error interno.");
        }
    }

    /// <summary>
    /// Fuerza la reclasificación de todos los usuarios del tenant.
    /// </summary>
    /// <remarks>
    /// Esta operación puede ser intensiva en recursos. Usar con precaución.
    /// </remarks>
    [HttpPost("recalcular-segmentos")]
    [ProducesResponseType(202)] // Accepted (la operación se inició)
    public async Task<IActionResult> RecalcularSegmentos(Guid tenantId)
    {
        tenantId = GetEffectiveTenantId(tenantId);
        _logger.LogInformation("API: Solicitud para recalcular segmentos para TenantId {TenantId}", tenantId);
        // No esperamos que termine aquí, solo que se inicie.
        // Para una operación larga, considera usar un sistema de colas/trabajos en segundo plano.
       
        await _audienciaService.ActualizarSegmentosUsuariosAsync(tenantId, null); // Se puede pasar una lista de usuarios para segmentar, o null para todos los usuarios.
        _logger.LogInformation("API: Recalculación de segmentos iniciada para TenantId {TenantId}", tenantId);
        return Accepted();
    }

    /// <summary>
    /// Obtiene la lista de usuarios que pertenecen a una audiencia específica.
    /// </summary>
    /// <param name="tenantId">ID del tenant.</param>
    /// <param name="nombreUnicoAudiencia">El NombreUnicoInterno de la audiencia.</param>
    /// <returns>Una lista de usuarios.</returns>
    [HttpGet("{nombreUnicoAudiencia}/usuarios")]
    [ProducesResponseType(typeof(IEnumerable<Usuario>), 200)]
    [ProducesResponseType(404)] // Si la audiencia no existe
    public async Task<IActionResult> GetUsuariosDeAudiencia(Guid tenantId, string nombreUnicoAudiencia)
    {
        tenantId = GetEffectiveTenantId(tenantId);
        _logger.LogInformation("API: Solicitud para obtener usuarios de la audiencia '{NombreUnico}' para TenantId {TenantId}", nombreUnicoAudiencia, tenantId);

        if (string.IsNullOrWhiteSpace(nombreUnicoAudiencia))
        {
            return BadRequest("El nombre único de la audiencia es requerido.");
        }

        var usuarios = await _audienciaService.GetUsuariosPorAudienciaAsync(tenantId, nombreUnicoAudiencia);

        // GetUsuariosPorAudienciaAsync podría devolver vacío si la audiencia no existe o no tiene usuarios.
        // Podrías añadir una verificación explícita si la audiencia existe si quieres un 404 más preciso
        // cuando la audiencia en sí no se encuentra, versus cuando simplemente no tiene usuarios.
        // var audienciaDef = (await _audienciaService.GetDefinicionesAudienciaAsync(tenantId))
        //                  .FirstOrDefault(a => a.NombreUnicoInterno.Equals(nombreUnicoAudiencia, StringComparison.OrdinalIgnoreCase));
        // if (audienciaDef == null && !nombreUnicoAudiencia.Equals(KeyParaUsuariosNoAsignados, StringComparison.OrdinalIgnoreCase) ) // Asumiendo KeyParaUsuariosNoAsignados
        // {
        //     return NotFound($"Audiencia '{nombreUnicoAudiencia}' no encontrada.");
        // }

        return Ok(usuarios);
    }

    /// <summary>
    /// Obtiene la distribución de usuarios por cada audiencia (NombreUnicoInterno y conteo).
    /// </summary>
    [HttpGet("distribucion")]
    [ProducesResponseType(typeof(Dictionary<string, int>), 200)]
    public async Task<IActionResult> GetDistribucion(Guid tenantId)
    {
        tenantId = GetEffectiveTenantId(tenantId);
        _logger.LogInformation("API: Solicitud para obtener distribución de audiencias para TenantId {TenantId}", tenantId);
        var distribucion = await _audienciaService.GetDistribucionUsuariosPorAudienciaAsync(tenantId);
        return Ok(distribucion);
    }

    /// <summary>
    /// Obtiene estadísticas globales y por audiencia.
    /// </summary>
    [HttpGet("estadisticas")]
    [ProducesResponseType(typeof(EstadisticasAudienciaDinamica), 200)]
    public async Task<IActionResult> GetEstadisticas(Guid tenantId)
    {
        tenantId = GetEffectiveTenantId(tenantId);
        _logger.LogInformation("API: Solicitud para obtener estadísticas de audiencias para TenantId {TenantId}", tenantId);
        var estadisticas = await _audienciaService.GetEstadisticasGlobalesYAporAudienciaAsync(tenantId);
        return Ok(estadisticas);
    }

    /// <summary>
    /// Clasifica un usuario específico y devuelve el NombreUnicoInterno de la audiencia a la que pertenece.
    /// </summary>
    [HttpGet("usuarios/{usuarioId}/clasificar")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(404)] // Si el usuario no existe
    public async Task<IActionResult> ClasificarUsuario(Guid tenantId, Guid usuarioId)
    {
        tenantId = GetEffectiveTenantId(tenantId);
        _logger.LogInformation("API: Solicitud para clasificar UsuarioId {UsuarioId} en TenantId {TenantId}", usuarioId, tenantId);
        try
        {
            string nombreUnicoAudiencia = await _audienciaService.ClasificarUsuarioAsync(usuarioId, tenantId);
            if (nombreUnicoAudiencia == null)
            {
                // Esto significa que el usuario no cayó en ninguna audiencia específica y no hay un "default" con nombre
                // o el servicio devuelve null para el default implícito.
                return Ok(new { usuarioId = usuarioId, audienciaAsignada = (string)null, mensaje = "Usuario no asignado a una audiencia específica o pertenece al segmento por defecto implícito." });
            }
            return Ok(new { usuarioId = usuarioId, audienciaAsignada = nombreUnicoAudiencia });
        }
        catch (KeyNotFoundException ex) // Si el usuario no se encuentra en ClasificarUsuarioAsync (aunque nuestro servicio actual devuelve null)
        {
            _logger.LogWarning(ex, "API: UsuarioId {UsuarioId} no encontrado al clasificar en TenantId {TenantId}", usuarioId, tenantId);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API: Error inesperado al clasificar UsuarioId {UsuarioId} en TenantId {TenantId}", usuarioId, tenantId);
            return StatusCode(500, "Ocurrió un error interno.");
        }
    }
}