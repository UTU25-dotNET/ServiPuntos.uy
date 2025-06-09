using ServiPuntos.Core.DTOs;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _iUsuarioRepository;

    private readonly ITenantResolver _iTenantResolver;

    private readonly ITenantContext _iTenantContext;

    private readonly IConfigPlataformaService _configPlataformaService;

    public UsuarioService(
        IUsuarioRepository usuarioRepository,
        ITenantResolver tenantResolver,
        ITenantContext tenantContext,
        IConfigPlataformaService configPlataformaService)
    {
        _iUsuarioRepository = usuarioRepository;
        _iTenantResolver = tenantResolver;
        _iTenantContext = tenantContext;
        _configPlataformaService = configPlataformaService;
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
        if (usuario == null)
        {
            return null;
        }

        var config = await _configPlataformaService.ObtenerConfiguracionAsync();
        int maxIntentos = config?.MaximoIntentosLogin ?? 3;

        if (usuario.Bloqueado)
        {
            return null;
        }

        if (!usuario.VerificarPassword(password))
        {
            usuario.IntentosFallidos++;
            if (usuario.IntentosFallidos >= maxIntentos)
            {
                usuario.Bloqueado = true;
            }
            await _iUsuarioRepository.UpdateAsync(usuario);
            return null;
        }

        usuario.IntentosFallidos = 0;
        usuario.Bloqueado = false;
        await _iUsuarioRepository.UpdateAsync(usuario);
        return usuario;
    }

}