using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public interface IUbicacionService
    {
        Task<List<Ubicacion>> GetUbicacionesTenantAsync(string tenantId);

        Task<List<Ubicacion>> GetAllAsync();
    }

    public class UbicacionService : IUbicacionService
    {
        private readonly HttpClient _httpClient;

        public UbicacionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<List<Ubicacion>> GetUbicacionesTenantAsync(string tenantId)
            => _httpClient.GetFromJsonAsync<List<Ubicacion>>($"tenant/{tenantId}");



        public Task<List<Ubicacion>> GetAllAsync()
            => _httpClient.GetFromJsonAsync<List<Ubicacion>>("");
    }
}
