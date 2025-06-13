using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public interface IUserService
    {
        Task<Usuario?> GetPerfilByEmailAsync(string email);
        Task<Usuario?> GetPerfilByIdAsync(string id);
        Task<bool> UpdatePerfilAsync(Usuario usuario);

    }

    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Usuario?> GetPerfilByEmailAsync(string email)
            => await _httpClient.GetFromJsonAsync<Usuario>($"email/{email}");

        public async Task<Usuario?> GetPerfilByIdAsync(string id)
            => await _httpClient.GetFromJsonAsync<Usuario>($"{id}");

        public async Task<bool> UpdatePerfilAsync(Usuario usuario)
        {
            var resp = await _httpClient.PutAsJsonAsync($"{usuario.Id}", usuario);
            return resp.IsSuccessStatusCode;
        }


    }
}
