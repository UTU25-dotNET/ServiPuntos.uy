using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/usuario")]
public class UsuarioController : ControllerBase
{
    private readonly UsuarioService _usuarioService;

    public UsuarioController(UsuarioService service)
    {
        _usuarioService = service;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var usuario = await _usuarioService.ObtenerPorId(id);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _usuarioService.ListarUsuarios());
}
