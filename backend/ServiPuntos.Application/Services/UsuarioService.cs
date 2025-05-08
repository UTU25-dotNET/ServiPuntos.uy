using ServiPuntos.Application.DTOs;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _iUsuarioRepository;

    private readonly ITenantResolver _iTenantResolver;

    public UsuarioService(IUsuarioRepository usuarioRepository, ITenantResolver tenantResolver)
    {
        _iUsuarioRepository = usuarioRepository;
        _iTenantResolver = tenantResolver;
    }

    public async Task<Usuario?> GetUsuarioByIdAsync(Guid idUsuario)
    {
        return await _iUsuarioRepository.GetAsync(idUsuario);
    }
    /*public async Task<IEnumerable<Usuario>> GetAllUsuariosAsync()
    {
        return await _iUsuarioRepository.GetAllAsync();
    }*/
    public async Task AddUsuarioAsync(Usuario usuario)
    {
        var tenantId = _iTenantResolver.GetCurrentTenantId();
        await _iUsuarioRepository.AddAsync(tenantId, usuario);
    }
    public async Task UpdateUsuarioAsync(Usuario usuario)
    {
        var tenantId = _iTenantResolver.GetCurrentTenantId();
        await _iUsuarioRepository.UpdateAsync(tenantId, usuario);
    }
    public async Task DeleteUsuarioAsync(Guid idUsuario)
    {
        var tenantId = _iTenantResolver.GetCurrentTenantId();
        await _iUsuarioRepository.DeleteAsync(tenantId, idUsuario);
    }
    // Tenant como parametro
    public async Task AddUsuarioByTenantAsync(Guid tenantId, Usuario usuario)
    {
        await _iUsuarioRepository.AddAsync(tenantId, usuario);
    }
    public async Task UpdateUsuarioByTenantAsync(Guid tenantId, Usuario usuario)
    {
        await _iUsuarioRepository.UpdateAsync(tenantId, usuario);
    }
    public async Task DeleteUsuarioByTenantAsync(Guid tenantId, Guid idUsuario)
    {
        await _iUsuarioRepository.DeleteAsync(tenantId, idUsuario);
    }
    public async Task<Usuario?> GetUsuarioByTenantAsync(Guid tenantId, Guid idUsuario)
    {
        return await _iUsuarioRepository.GetByTenantAsync(tenantId, idUsuario);
    }

    // Filtrar usuarios por tenant
    public async Task<IEnumerable<Usuario>> GetAllUsuariosByTenantAsync()
    {
        var tenantId = _iTenantResolver.GetCurrentTenantId();

        return await _iUsuarioRepository.GetAllByTenantAsync(tenantId);

    }
  
}
