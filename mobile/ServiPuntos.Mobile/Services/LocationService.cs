using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public class LocationService : ILocationService
    {
        private readonly HttpClient _httpClient;
        public LocationService(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<List<LocationDto>> GetLocationsByTenantAsync(Guid tenantId)
        {
            var url = $"/api/ubicacion/tenant/{tenantId}";
            Console.WriteLine($"[LocationService] GET {url}");
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            Console.WriteLine("[LocationService] Request headers:");
            foreach (var header in request.Headers)
                Console.WriteLine($"  {header.Key}: {string.Join(",", header.Value)}");
            var response = await _httpClient.SendAsync(request);
            Console.WriteLine($"[LocationService] Response status: {response.StatusCode}");
            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[LocationService] Response content: {json}");
            response.EnsureSuccessStatusCode();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var list = JsonSerializer.Deserialize<List<LocationDto>>(json, options);
            return list ?? new List<LocationDto>();
        }
    }
}
