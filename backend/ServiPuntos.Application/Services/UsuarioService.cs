using ServiPuntos.Application.DTOs;
using ServiPuntos.Core.Interfaces;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _iUsuarioRepository;

    private readonly ITenantResolver _iTenantResolver;

    private readonly ITenantContext _tenantContext;

    public UsuarioService(IUsuarioRepository usuarioRepository, ITenantResolver tenantResolver, ITenantContext tenantContext)
    {
        _iUsuarioRepository = usuarioRepository;
        _iTenantResolver = tenantResolver;
        _tenantContext = tenantContext;
    }

    public async Task<Usuario?> GetUsuarioByIdAsync(Guid idUsuario)
    {
        return await _iUsuarioRepository.GetAsync(idUsuario);
    }
    public async Task<IEnumerable<Usuario>> GetAllUsuariosAsync()
    {
        var tenantId = _tenantContext.TenantId;
        return await _iUsuarioRepository.GetAllByTenantAsync(tenantId);
    }
    public async Task AddUsuarioAsync(Usuario usuario)
    {
        var tenantId = _tenantContext.TenantId;
        await _iUsuarioRepository.AddAsync(tenantId, usuario);
    }
    public async Task UpdateUsuarioAsync(Usuario usuario)
    {
        var tenantId = _tenantContext.TenantId;
        await _iUsuarioRepository.UpdateAsync(tenantId, usuario);
    }
    public async Task DeleteUsuarioAsync(Guid idUsuario)
    {
        var tenantId = _tenantContext.TenantId;
        await _iUsuarioRepository.DeleteAsync(tenantId, idUsuario);
    }
    // Tenant como parametro (creo que ya no hace falta)
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
  
    // Validar credenciales
    public async Task<Usuario?> ValidarCredencialesAsync(string email, string password)
    {
        var usuario = await _iUsuarioRepository.GetByEmailAsync(email);
        if (usuario == null || !usuario.VerificarPassword(password))
        {
            return null;
        }
        return usuario;
    }

}
