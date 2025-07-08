using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServiPuntos.Mobile.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task UpdateFcmTokenAsync(string token)
        {
            var json = JsonSerializer.Serialize(new { token });
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("/api/usuario/fcm", content);
            response.EnsureSuccessStatusCode();
        }
    }
}