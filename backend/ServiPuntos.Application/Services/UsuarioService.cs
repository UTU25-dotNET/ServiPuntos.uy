using ServiPuntos.Application.DTOs;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _iUsuarioRepository;

    private readonly ITenantResolver _iTenantResolver;

    private readonly ITenantContext _iTenantContext;

    public UsuarioService(IUsuarioRepository usuarioRepository, ITenantResolver tenantResolver, ITenantContext tenantContext)
    {
        _iUsuarioRepository = usuarioRepository;
        _iTenantResolver = tenantResolver;
        _iTenantContext = tenantContext;
    }

    //GET

    public async Task<Usuario?> GetUsuarioAsync(Guid idUsuario)
    {
        return await _iUsuarioRepository.GetAsync(idUsuario);
    }
    public async Task<Usuario?> GetUsuarioAsync(string email)
    {
        return await _iUsuarioRepository.GetByEmailAsync(email);
    }
    public async Task<Usuario?> GetUsuarioAsync(Guid tenantId, Guid idUsuario)
    {
        return await _iUsuarioRepository.GetByTenantAsync(tenantId, idUsuario);
    }

    //GET ALL 
    public async Task<IEnumerable<Usuario>> GetAllUsuariosAsync()
    {
        return await _iUsuarioRepository.GetAllAsync();
    }    
    
    public async Task<IEnumerable<Usuario>> GetAllUsuariosAsync(Guid tenantId)
    {
        //var tenantId = _tenantContext.TenantId;
        return await _iUsuarioRepository.GetAllByTenantAsync(tenantId);
    }

    //ADD

    public async Task AddUsuarioAsync(Usuario usuario)
    {
        //var tenantId = _tenantContext.TenantId;
        await _iUsuarioRepository.AddAsync(usuario);
    }
    public async Task AddUsuarioAsync(Guid tenantId, Usuario usuario)
    {
        await _iUsuarioRepository.AddAsync(tenantId, usuario);
    }

    //UPDATE
    public async Task UpdateUsuarioAsync(Usuario usuario)
    {
        //var tenantId = _tenantContext.TenantId;
        await _iUsuarioRepository.UpdateAsync(usuario);
    }
    public async Task UpdateUsuarioByTenantAsync(Guid tenantId, Usuario usuario)
    {
        await _iUsuarioRepository.UpdateAsync(tenantId, usuario);
    }

    //DELETE
    public async Task DeleteUsuarioAsync(Guid idUsuario)
    {
        //var tenantId = _tenantContext.TenantId;
        await _iUsuarioRepository.DeleteAsync(idUsuario);
    }

    public async Task DeleteUsuarioAsync(Guid tenantId, Guid idUsuario)
    {
        await _iUsuarioRepository.DeleteAsync(tenantId, idUsuario);
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
