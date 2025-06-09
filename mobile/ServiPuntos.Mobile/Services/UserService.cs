using ServiPuntos.Mobile.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiPuntos.Mobile.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private const string BASE_URL = "https://ec2-18-220-251-96.us-east-2.compute.amazonaws.com:5019/api/usuario";

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Usuario?> GetPerfilByEmailAsync(string email)
        {
            return await _httpClient.GetFromJsonAsync<Usuario>($"{BASE_URL}/email/{email}");
        }


        public async Task<Usuario?> GetPerfilByIdAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<Usuario>($"{BASE_URL}/{id}");
        }


        public async Task<bool> UpdatePerfilAsync(Usuario usuario)
        {
            var resp = await _httpClient.PutAsJsonAsync($"{BASE_URL}/{usuario.Id}", usuario);
            return resp.IsSuccessStatusCode;
        }


        public async Task<PointBalanceResponse?> GetBalanceAsync()
        {
            return await _httpClient.GetFromJsonAsync<PointBalanceResponse>($"{BASE_URL}/saldo");
        }

        public async Task<List<TransactionSummary>?> GetRecentTransactionsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<TransactionSummary>>($"{BASE_URL}/historial?limit=5");
        }
    }
}
