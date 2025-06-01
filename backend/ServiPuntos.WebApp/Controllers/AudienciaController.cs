using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Core.DTOs;

/*
namespace ServiPuntos.WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AudienciaController : ControllerBase
    {
        private readonly IAudienciaService _audienciaService;

        public AudienciaController(IAudienciaService audienciaService)
        {
            _audienciaService = audienciaService;
        }

        [HttpGet]
        public async Task<ActionResult<List<AudienciaResumenDto>>> ObtenerAudiencias()
        {
            var audiencias = await _audienciaService.ObtenerAudienciasPorTenantAsync(TenantId);

            var resultado = new List<AudienciaResumenDto>();
            foreach (var audiencia in audiencias)
            {
                var cantidadUsuarios = await _audienciaService.ContarUsuariosDeAudienciaAsync(audiencia.Id);
                resultado.Add(new AudienciaResumenDto
                {
                    Id = audiencia.Id,
                    Nombre = audiencia.Nombre,
                    Descripcion = audiencia.Descripcion,
                    CantidadUsuarios = cantidadUsuarios,
                    FechaCreacion = audiencia.FechaCreacion,
                    Activa = audiencia.Activa,
                    Reglas = audiencia.Reglas.Select(r => new ReglaResumenDto
                    {
                        Campo = r.Campo,
                        Operador = r.Operador,
                        Valor = r.Valor,
                        OperadorLogico = r.OperadorLogico,
                        DescripcionHumana = GenerarDescripcionHumana(r)
                    }).ToList()
                });
            }

            return Ok(resultado);
        }

        [HttpPost]
        public async Task<ActionResult<Audiencia>> CrearAudiencia([FromBody] CrearAudienciaDto dto)
        {
            try
            {
                var audiencia = await _audienciaService.CrearAudienciaAsync(dto);
                return CreatedAtAction(nameof(ObtenerAudiencia), new { id = audiencia.Id }, audiencia);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Audiencia>> ObtenerAudiencia(int id)
        {
            var audiencia = await _audienciaService.ObtenerAudienciaPorIdAsync(id);
            if (audiencia == null)
                return NotFound();

            return Ok(audiencia);
        }

        [HttpGet("{id}/usuarios")]
        public async Task<ActionResult<List<Usuario>>> ObtenerUsuariosAudiencia(int id)
        {
            var usuarios = await _audienciaService.ObtenerUsuariosDeAudienciaAsync(id);
            return Ok(usuarios);
        }

        [HttpGet("{id}/usuarios/contar")]
        public async Task<ActionResult<int>> ContarUsuariosAudiencia(int id)
        {
            var cantidad = await _audienciaService.ContarUsuariosDeAudienciaAsync(id);
            return Ok(cantidad);
        }

        [HttpGet("campos-disponibles")]
        public async Task<ActionResult<List<CampoDisponible>>> ObtenerCamposDisponibles()
        {
            var campos = await _audienciaService.ObtenerCamposDisponiblesAsync();
            return Ok(campos);
        }

        [HttpGet("operadores/{tipoDato}")]
        public async Task<ActionResult<List<OperadorDisponible>>> ObtenerOperadores(string tipoDato)
        {
            var operadores = await _audienciaService.ObtenerOperadoresPorTipoAsync(tipoDato);
            return Ok(operadores);
        }

        private string GenerarDescripcionHumana(ReglaAudiencia regla)
        {
            return regla.Campo switch
            {
                "Edad" when regla.Operador == "Mayor" => $"Usuarios mayores de {regla.Valor} años",
                "Edad" when regla.Operador == "Menor" => $"Usuarios menores de {regla.Valor} años",
                "Ubicacion" when regla.Operador == "Igual" => $"Usuarios de {regla.Valor}",
                "TotalCompras" when regla.Operador == "Mayor" => $"Usuarios con compras mayores a ${regla.Valor}",
                _ => $"{regla.Campo} {regla.Operador} {regla.Valor}"
            };
        }

        private int TenantId => int.Parse(User.FindFirst("TenantId")?.Value ?? "0");
    }
}*/
