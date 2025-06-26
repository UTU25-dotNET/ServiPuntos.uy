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
            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[LocationService] StatusCode={(int)response.StatusCode}, Content={json}");
            response.EnsureSuccessStatusCode();
            return JsonSerializer.Deserialize<List<LocationDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<LocationDto>();
        }

        public async Task<List<LocationDto>> GetAllLocationsAsync()
        {
            var url = "/api/ubicacion";
            Console.WriteLine($"[LocationService] GET {url}");
            var response = await _httpClient.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[LocationService] StatusCode={(int)response.StatusCode}, Content={json}");
            response.EnsureSuccessStatusCode();
            return JsonSerializer.Deserialize<List<LocationDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<LocationDto>();
        }

        public async Task<List<LocationDto>> GetNearbyAsync(
            double latitude,
            double longitude,
            string? servicio = null,
            string? combustible = null,
            int radioMetros = 1000)
        {
            var url = $"/api/ubicacion/cercanas?lat={latitude}&lon={longitude}&radioMetros={radioMetros}"
                    + (servicio != null ? $"&servicio={Uri.EscapeDataString(servicio)}" : "")
                    + (combustible != null ? $"&combustible={Uri.EscapeDataString(combustible)}" : "");
            Console.WriteLine($"[LocationService] GET {_httpClient.BaseAddress}{url}");
            var response = await _httpClient.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[LocationService] StatusCode={(int)response.StatusCode}, Content={json}");
            response.EnsureSuccessStatusCode();
            return JsonSerializer.Deserialize<List<LocationDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<LocationDto>();
        }
    }
}
