using ServiPuntos.Core.Entities;

namespace ServiPuntos.Core.Interfaces
{
    public interface IDispositivoRepository
    {
        Task AddAsync(DispositivoFcm dispositivo);
        Task<List<DispositivoFcm>> GetByUsuarioAsync(Guid usuarioId);
        Task<List<DispositivoFcm>> GetByUsuariosAsync(IEnumerable<Guid> usuarioIds);
    }
}
