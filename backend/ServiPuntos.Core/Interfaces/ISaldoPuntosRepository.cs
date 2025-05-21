using System.Threading.Tasks;
using ServiPuntos.Core.Entities;

namespace ServiPuntos.Core.Interfaces
{
    public interface ISaldoPuntosRepository
    {
        Task<SaldoPuntos> GetByUsuarioIdAndTenantIdAsync(int usuarioId, int tenantId);
        Task<int> AddAsync(SaldoPuntos saldoPuntos);
        Task<bool> UpdateAsync(SaldoPuntos saldoPuntos);
        Task<bool> DeleteAsync(int id);
    }
}