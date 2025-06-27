using System;
using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public interface IHistoryService
    {
        Task<PaginatedResponse<TransaccionDto>> GetHistoryAsync(Guid? cursor = null, int limit = 10);
    }
}
