using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _iUsuarioService;

    public UsuarioController(IUsuarioService service)
    {
        _iUsuarioService = service;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var usuario = await _iUsuarioService.GetUsuarioByIdAsync(id);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _iUsuarioService.GetAllUsuariosByTenantAsync());
}
