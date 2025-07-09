using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;
using static ServiPuntos.Mobile.Services.AppLogger;

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

        public async Task<bool> SetTokenFcmAsync(string token)
        {
            try
            {
                var payload = new { token };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync("/api/usuario/fcm", content);

                if (response.IsSuccessStatusCode)
                {
                    LogInfo("[NotificationService] Token FCM enviado correctamente al backend.");
                    return true;
                }
                else
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    LogInfo($"[NotificationService] Error al enviar token FCM: {response.StatusCode} - {errorBody}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogInfo($"[NotificationService] Excepción al enviar token FCM: {ex.Message}");
                return false;
            }
        }
    }
}
