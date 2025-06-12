using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    // Refit API interface para UbicacionController
    public interface IUbicacionApi
    {
        // GET api/ubicacion/tenant/{tenantId}
        [Get("/tenant/{tenantId}")]
        Task<List<Ubicacion>> GetByTenantAsync(string tenantId);

        // GET api/ubicacion/nearby?lat={lat}&lng={lng}&radius={radiusKm}
        [Get("/nearby")]
        Task<IEnumerable<UbicacionDto>> GetNearbyAsync(
            [AliasAs("lat")] double latitude,
            [AliasAs("lng")] double longitude,
            [AliasAs("radius")] double radiusKm);

        // GET api/ubicacion
        [Get("")]
        Task<List<Ubicacion>> GetAllAsync();
    }

    // Servicio y su interfaz combinados
    public interface IUbicacionService
    {
        Task<List<Ubicacion>> GetUbicacionesTenantAsync(string tenantId);
        Task<IEnumerable<UbicacionDto>> GetNearbyAsync(double lat, double lng, double radiusKm);
        Task<List<Ubicacion>> GetAllAsync();
    }

    public class UbicacionService : IUbicacionService
    {
        private readonly IUbicacionApi _api;

        public UbicacionService(IUbicacionApi api)
        {
            _api = api;
        }

        public async Task<List<Ubicacion>> GetUbicacionesTenantAsync(string tenantId)
        {
            try
            {
                return await _api.GetByTenantAsync(tenantId);
            }
            catch (ApiException)
            {
                return new List<Ubicacion>();
            }
        }

        public async Task<IEnumerable<UbicacionDto>> GetNearbyAsync(double lat, double lng, double radiusKm)
        {
            try
            {
                return await _api.GetNearbyAsync(lat, lng, radiusKm);
            }
            catch (ApiException)
            {
                return Array.Empty<UbicacionDto>();
            }
        }

        public async Task<List<Ubicacion>> GetAllAsync()
        {
            try
            {
                return await _api.GetAllAsync();
            }
            catch (ApiException)
            {
                return new List<Ubicacion>();
            }
        }
    }
}
