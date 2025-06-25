using System;
using System.Threading.Tasks;

namespace ServiPuntos.Mobile.Services
{
    public interface ICanjeService
    {
        Task<string> GenerarCanjeAsync(Guid usuarioId, Guid productoId, Guid ubicacionId, Guid tenantId);
    }
}
