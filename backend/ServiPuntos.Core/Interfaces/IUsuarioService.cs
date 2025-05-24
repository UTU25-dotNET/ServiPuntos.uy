using ServiPuntos.Core.Entities;

public interface IUsuarioService
{
    // GET
    Task<Usuario?> GetUsuarioAsync(Guid idUsuario);
    Task<Usuario?> GetUsuarioAsync(string email);
    Task<Usuario?> GetUsuarioAsync(Guid tenantId, Guid idUsuario);
    Task<IEnumerable<Usuario>> GetAllUsuariosAsync();
    Task<IEnumerable<Usuario>> GetAllUsuariosAsync(Guid tenantId);

    // ADD
    Task AddUsuarioAsync(Usuario usuario);
    Task AddUsuarioAsync(Guid tenantId, Usuario usuario);

    // UPDATE
    Task UpdateUsuarioAsync(Usuario usuario);
    Task UpdateUsuarioByTenantAsync(Guid tenantId, Usuario usuario);

    // DELETE
    Task DeleteUsuarioAsync(Guid idUsuario);
    Task DeleteUsuarioAsync(Guid tenantId, Guid idUsuario);

    // LOGIN
    Task<Usuario?> ValidarCredencialesAsync(string email, string password);
}
