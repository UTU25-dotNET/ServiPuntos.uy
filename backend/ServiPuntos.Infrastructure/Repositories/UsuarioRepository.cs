using Microsoft.EntityFrameworkCore;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Infrastructure.Data;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly ServiPuntosDbContext _dbContext;

    public UsuarioRepository(ServiPuntosDbContext context)
    {
        _dbContext = context;
    }

    Task<Usuario?> GetAsync(Guid idUsuario)
        => _dbContext.Usuarios
            .FirstOrDefaultAsync(u => u.Id == idUsuario);
    Task AddAsync(Usuario usuario)
    {
        _dbContext.Usuarios.Add(usuario);
        return _dbContext.SaveChangesAsync();
    }

    Task UpdateAsync(Usuario usuario)
    {
        _dbContext.Usuarios.Update(usuario);
        return _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid idUsuario)
    {

    }

    // Tenant como parametro

    public async Task<Usuario?> GetByTenantAsync(Guid tenantId, Guid idUsuario)
        => await _dbContext.Usuarios
            .FirstOrDefaultAsync(u => u.TenantId == tenantId & u.Id == idUsuario);
    public async Task<IEnumerable<Usuario>> GetAllByTenantAsync(Guid tenantId)
    => await _dbContext.Usuarios
        .Where(u => u.TenantId == tenantId)
        .ToListAsync();
    public async Task AddAsync(Guid tenantId, Usuario usuario)
    {
        usuario.TenantId = tenantId;
        await _dbContext.Usuarios.AddAsync(usuario);
        await _dbContext.SaveChangesAsync();
    }
    public async Task UpdateAsync(Guid tenantId, Usuario usuario)
    {
        usuario.TenantId = tenantId;
        _dbContext.Usuarios.Update(usuario);
        await _dbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Guid tenantId, Guid idUsuario)
    {
        var usuario = await _dbContext.Usuarios
            .FirstOrDefaultAsync(u => u.TenantId == tenantId && u.Id == idUsuario);
        if (usuario != null)
        {
            _dbContext.Usuarios.Remove(usuario);
            await _dbContext.SaveChangesAsync();
        }
    }
}
