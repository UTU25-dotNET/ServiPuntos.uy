using Microsoft.AspNetCore.Mvc;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;

/// <summary>
/// Exposes the current platform configuration set by the AdminPlataforma.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ConfigPlataformaController : ControllerBase
{
    private readonly IConfigPlataformaService _configService;

    public ConfigPlataformaController(IConfigPlataformaService configService)
    {
        _configService = configService;
    }

    /// <summary>
    /// Returns the global platform configuration.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ConfigPlataforma>> Get()
    {
        var config = await _configService.ObtenerConfiguracionAsync();
        if (config == null)
        {
            return NotFound();
        }

        return Ok(config);
    }
}