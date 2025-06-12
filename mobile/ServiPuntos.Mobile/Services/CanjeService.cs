using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public interface ICanjeService
    {
        Task<CanjeResponse> GenerarCanjeAsync(CanjeRequest body);
        Task<IEnumerable<CanjeHistorialItem>> GetHistorialAsync(string userId);

        Task<bool> ValidateQrAsync(string codigoQr);

    }

    public class CanjeService : ICanjeService
    {
        private readonly HttpClient _httpClient;

        public CanjeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CanjeResponse> GenerarCanjeAsync(CanjeRequest body)
        {
            var response = await _httpClient.PostAsJsonAsync("generar-canje", body);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CanjeResponse>();
        }

        public async Task<IEnumerable<CanjeHistorialItem>> GetHistorialAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"usuario/{userId}");

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<CanjeHistorialItem>>() ?? new List<CanjeHistorialItem>();
        }

        public async Task<bool> ValidateQrAsync(string codigoQr)
        {
            var resp = await _httpClient.GetAsync($"validar/{codigoQr}");
            return resp.IsSuccessStatusCode;
        }

    }
}
