using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public class TenantService : ITenantService
    {
        private readonly HttpClient _httpClient;

        public TenantService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TenantConfig> GetByIdAsync(Guid tenantId)
        {
            var response = await _httpClient.GetAsync($"api/tenant/{tenantId}");
            var raw = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"[TenantService] GET /api/tenant/{tenantId} → {(int)response.StatusCode} {raw}");
            response.EnsureSuccessStatusCode();
            var dto = JsonSerializer.Deserialize<TenantDto>(raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            System.Diagnostics.Debug.WriteLine($"[TenantService] Fetched tenant color: {dto.Color}");
            return new TenantConfig
            {
                Id = dto.Id.ToString(),
                Name = dto.Nombre,
                LogoUrl = dto.LogoUrl ?? string.Empty,
                PrimaryColor = dto.Color ?? "#512BD4",
                SecondaryColor = "#FFFFFF"
            };
        }




        private class TenantDto
        {
            public Guid Id { get; set; }
            public string Nombre { get; set; } = default!;
            public string? LogoUrl { get; set; }
            public string? Color { get; set; }
        }
    }
}
