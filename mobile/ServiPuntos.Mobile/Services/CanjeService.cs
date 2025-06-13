using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public interface ICanjeService
    {
        Task<RespuestaNAFTA> GenerarCanjeAsync(MensajeNAFTA body);
        Task<IEnumerable<CanjeHistorialItem>> GetHistorialAsync(string userId);

    }

    public class CanjeService : ICanjeService
    {
        private readonly HttpClient _httpClient;

        public CanjeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<CanjeHistorialItem>> GetHistorialAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"usuario/{userId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<CanjeHistorialItem>>() ?? new List<CanjeHistorialItem>();
        }

        public async Task<RespuestaNAFTA> GenerarCanjeAsync(MensajeNAFTA body)
        {
            var response = await _httpClient.PostAsJsonAsync("generar-canje", body);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<RespuestaNAFTA>();
        }
    }
}
