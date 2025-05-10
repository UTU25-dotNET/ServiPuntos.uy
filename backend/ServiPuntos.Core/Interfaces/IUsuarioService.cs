public interface IUsuarioService
{
    Task<Usuario?> GetUsuarioByIdAsync(Guid id);
    //Task<IEnumerable<Usuario>> GetAllUsuariosAsync();
    Task AddUsuarioAsync(Usuario usuario);
    Task UpdateUsuarioAsync(Usuario usuario);
    Task DeleteUsuarioAsync(Guid id);

    Task<Usuario?> GetUsuarioByTenantAsync(Guid tenantId, Guid idUsuario);
    Task<IEnumerable<Usuario>> GetAllUsuariosByTenantAsync();
    Task AddUsuarioByTenantAsync(Guid tenantId, Usuario usuario);
    Task UpdateUsuarioByTenantAsync(Guid tenantId, Usuario usuario);
    Task DeleteUsuarioByTenantAsync(Guid tenantId, Guid idUsuario);

    //Usado por Backoffice en WebApp con cookies
    Task<Usuario?> ValidarCredencialesAsync(string email, string password);
}