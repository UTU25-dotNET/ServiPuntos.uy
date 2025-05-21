using System.Threading.Tasks;
using ServiPuntos.Core.Entities;

namespace ServiPuntos.Core.Interfaces
{
    public interface ISaldoPuntosRepository
    {
        Task<SaldoPuntos> GetByUsuarioIdAndTenantIdAsync(Guid usuarioId, Guid tenantId);
        Task<Guid> AddAsync(SaldoPuntos saldoPuntos);
        Task<bool> UpdateAsync(SaldoPuntos saldoPuntos);
        Task<bool> DeleteAsync(Guid id);
    }
}