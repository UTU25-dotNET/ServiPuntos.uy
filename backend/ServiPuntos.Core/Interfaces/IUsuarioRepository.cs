public interface IUsuarioRepository
{
    Task<Usuario?> GetAsync(Guid idUsuario);
    Task<Usuario?> GetByEmailAsync(string email);
    Task AddAsync(Usuario usuario);
    Task UpdateAsync(Usuario usuario);
    Task DeleteAsync(Guid idUsuario);
   
    //Tenant como parametro

    Task<Usuario?> GetByTenantAsync(Guid tenantId, Guid idUsuario);
    Task<IEnumerable<Usuario>> GetAllByTenantAsync(Guid tenantId);
    Task AddAsync(Guid tenantId, Usuario usuario);
    Task UpdateAsync(Guid tenantId, Usuario usuario);
    Task DeleteAsync(Guid tenantId, Guid idUsuario);
    
}