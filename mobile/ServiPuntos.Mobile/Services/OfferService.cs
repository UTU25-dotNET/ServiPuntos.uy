using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using ServiPuntos.Mobile.Models;
using static ServiPuntos.Mobile.Services.AppLogger;

namespace ServiPuntos.Mobile.Services
{
    public class OfferService : IOfferService
    {
        private readonly HttpClient _httpClient;
        public OfferService(HttpClient httpClient) =>
            _httpClient = httpClient;

        public async Task<List<OfferDto>> GetOffersAsync()
        {
            var tenantId = await SecureStorage.GetAsync("tenant_id");
            LogInfo($"[OfferService] GET /api/promocion/tenant/{tenantId}");
            Console.WriteLine($"[OfferService] Preparing request for /api/promocion/tenant/{tenantId}");
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/promocion/tenant/{tenantId}");
            Console.WriteLine("[OfferService] Request headers:");
            foreach (var header in request.Headers)
                Console.WriteLine($"[OfferService]   {header.Key}: {string.Join(",", header.Value)}");
            var response = await _httpClient.SendAsync(request);
            Console.WriteLine($"[OfferService] Response status: {response.StatusCode}");
            response.EnsureSuccessStatusCode();
            var raw = await response.Content.ReadAsStringAsync();
            LogInfo($"[OfferService] Response: {raw}");
            Console.WriteLine($"[OfferService] Response content: {raw}");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var list = JsonSerializer.Deserialize<List<OfferDto>>(raw, options);
            return list ?? new List<OfferDto>();
        }
    }
}
