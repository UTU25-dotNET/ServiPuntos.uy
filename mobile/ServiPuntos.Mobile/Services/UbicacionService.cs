
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public interface IUbicacionService
    {
        Task<List<Ubicacion>> GetUbicacionesTenantAsync(string tenantId);
        Task<IEnumerable<UbicacionDto>> GetNearbyAsync(double lat, double lng, double radiusKm);

        Task<List<Ubicacion>> GetAllAsync();

    }

    public class UbicacionService : IUbicacionService
    {
        private readonly HttpClient _httpClient;
        public UbicacionService(HttpClient httpClient) => _httpClient = httpClient;


        public Task<List<Ubicacion>> GetUbicacionesTenantAsync(string tenantId) =>
            _httpClient.GetFromJsonAsync<List<Ubicacion>>($"{tenantId}");

        public async Task<IEnumerable<UbicacionDto>> GetNearbyAsync(double lat, double lng, double radiusKm)
        {
            var url = $"nearby?lat={lat}&lng={lng}&radius={radiusKm}";
            var resp = await _httpClient.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<IEnumerable<UbicacionDto>>()
                   ?? Array.Empty<UbicacionDto>();
        }

        public Task<List<Ubicacion>> GetAllAsync() =>
            _httpClient.GetFromJsonAsync<List<Ubicacion>>("");


    }
}
