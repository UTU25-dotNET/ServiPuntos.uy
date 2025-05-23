using System.Threading.Tasks;

namespace ServiPuntos.Core.Interfaces
{
    public interface IPuntosService
    {
        Task<int> GetSaldoByUsuarioIdAsync(Guid usuarioId);
        Task ActualizarSaldoAsync(Guid usuarioId, int puntosAgregar);
        Task DebitarPuntosAsync(Guid usuarioId, int puntosDebitar);
    }
}