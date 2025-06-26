using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public interface ILocationService
    {
        Task<List<LocationDto>> GetLocationsByTenantAsync(Guid tenantId);
    }
}
