public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<UsuarioDto> GetUsuarioByIdAsync(int id)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id);
        if (usuario == null)
        {
            return null;
        }

        // Lógica de negocio adicional
        return new UsuarioDto
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Email = usuario.Email
        };
    }

    public async Task<IEnumerable<UsuarioDto>> GetAllUsuariosAsync()
    {
        var usuarios = await _usuarioRepository.GetAllAsync();
        return usuarios.Select(usuario => new UsuarioDto
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Email = usuario.Email
        });
    }

    public async Task AddUsuarioAsync(UsuarioDto usuarioDto)
    {
        var usuario = new Usuario
        {
            Nombre = usuarioDto.Nombre,
            Email = usuarioDto.Email
        };

        // Lógica de negocio adicional antes de guardar
        await _usuarioRepository.AddAsync(usuario);
    }

    public async Task UpdateUsuarioAsync(UsuarioDto usuarioDto)
    {
        var usuario = new Usuario
        {
            Id = usuarioDto.Id,
            Nombre = usuarioDto.Nombre,
            Email = usuarioDto.Email
        };

        // Lógica de negocio adicional antes de actualizar
        await _usuarioRepository.UpdateAsync(usuario);
    }

    public async Task DeleteUsuarioAsync(int id)
    {
        // Lógica de negocio adicional antes de eliminar
        await _usuarioRepository.DeleteAsync(id);
    }
}
