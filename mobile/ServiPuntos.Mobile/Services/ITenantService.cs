using System;
using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public interface ITenantService
    {
        Task<TenantConfig> GetByIdAsync(Guid tenantId);
    }
}
