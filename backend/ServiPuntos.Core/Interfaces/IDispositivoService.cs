namespace ServiPuntos.Core.Interfaces
{
    public interface IDispositivoService
    {
        Task RegistrarTokenAsync(Guid usuarioId, string token);
        Task<List<string>> GetTokensByUsuariosAsync(IEnumerable<Guid> usuarioIds);
    }
}
