using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public interface ILocationService
    {
        Task<List<LocationDto>> GetLocationsByTenantAsync(Guid tenantId);
        Task<List<LocationDto>> GetAllLocationsAsync();
        Task<List<LocationDto>> GetNearbyAsync(
            double latitude,
            double longitude,
            string? servicio = null,
            string? combustible = null,
            int radioMetros = 1000);
    }
}
