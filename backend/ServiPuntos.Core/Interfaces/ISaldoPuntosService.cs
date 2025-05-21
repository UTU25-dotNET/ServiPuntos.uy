using System.Threading.Tasks;
using ServiPuntos.Core.Entities;

namespace ServiPuntos.Core.Interfaces
{
    public interface ISaldoPuntosService
    {
        Task<decimal> GetSaldoByUsuarioIdAndTenantIdAsync(int usuarioId, int tenantId);
        Task<bool> ActualizarSaldoAsync(int usuarioId, int tenantId, decimal puntosAgregar);
        Task<bool> DebitarPuntosAsync(int usuarioId, int tenantId, decimal puntosDebitar);
    }
}