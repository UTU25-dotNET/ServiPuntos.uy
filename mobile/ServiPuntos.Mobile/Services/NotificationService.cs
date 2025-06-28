using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public class NotificationService : INotificationService
    {
        private readonly HttpClient _httpClient;
        public NotificationService(HttpClient httpClient) =>
            _httpClient = httpClient;

        public async Task<List<NotificationDto>> GetMyNotificationsAsync()
        {
            var response = await _httpClient.GetAsync("/api/notificacion/mine");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var list = JsonSerializer.Deserialize<List<NotificationDto>>(json, options);
            return list ?? new List<NotificationDto>();
        }
    }
}
