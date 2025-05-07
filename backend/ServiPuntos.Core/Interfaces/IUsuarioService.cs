public interface IUsuarioService
{
    Task<Usuario> GetUsuarioByIdAsync(int id);
    Task<IEnumerable<Usuario>> GetAllUsuariosAsync();
    Task AddUsuarioAsync(Usuario usuario);
    Task UpdateUsuarioAsync(Usuario usuario);
    Task DeleteUsuarioAsync(int id);
}