using System;
using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public interface ICanjeService
    {
        Task<string> GenerarCanjeAsync(Guid usuarioId, Guid productoId, Guid ubicacionId, Guid tenantId);
        Task<CanjeListResponseDto> GetCanjesByUsuarioAsync(Guid usuarioId, Guid? cursor = null, int limit = 20);
    }
}
