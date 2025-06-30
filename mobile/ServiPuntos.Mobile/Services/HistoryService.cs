using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly HttpClient _httpClient;

        public HistoryService(HttpClient httpClient) =>
            _httpClient = httpClient;

        public async Task<PaginatedResponse<TransaccionDto>> GetHistoryAsync(Guid? cursor = null, int limit = 10)
        {
            var token = await SecureStorage.GetAsync("auth_token");
            Console.WriteLine($"[HistoryService] Token presente: {!string.IsNullOrEmpty(token)}");

            var url = $"api/usuario/historial-transacciones?limit={limit}";
            if (cursor.HasValue)
                url += $"&cursor={cursor.Value}";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            Console.WriteLine($"[HistoryService] GET {url}");
            var response = await _httpClient.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[HistoryService] STATUS: {response.StatusCode} BODY: {body}");

            response.EnsureSuccessStatusCode();
            return JsonSerializer.Deserialize<PaginatedResponse<TransaccionDto>>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new PaginatedResponse<TransaccionDto>();
        }
    }
}
