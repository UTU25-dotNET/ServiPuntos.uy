using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public class VeaiService : IVeaiService
    {
        private readonly HttpClient _httpClient;
        public VeaiService(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<VeaiResponse?> VerifyAgeAsync(string cedula)
        {
            var resp = await _httpClient.GetAsync($"age_verify?cedula={cedula}");
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<VeaiResponse>();
        }
    }
}
