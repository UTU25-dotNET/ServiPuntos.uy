using ServiPuntos.Mobile.Models;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace ServiPuntos.Mobile.ViewModels
{
    public class SessionService
    {
        private const string TokenKey = "user_token";
        private const string NombreKey = "user_nombre";
        private const string RolKey = "user_rol";

        public async Task SaveSessionAsync(LoginResponse response)
        {
            if (response == null) return;

            await SecureStorage.SetAsync(TokenKey, response.Token ?? "");
            await SecureStorage.SetAsync(NombreKey, response.Nombre ?? "");
            await SecureStorage.SetAsync(RolKey, response.Rol ?? "");
        }

        public async Task<UserSession> GetSessionAsync()
        {
            var token = await SecureStorage.GetAsync(TokenKey);
            var nombre = await SecureStorage.GetAsync(NombreKey);
            var rol = await SecureStorage.GetAsync(RolKey);

            return new UserSession
            {
                Token = token,
                Nombre = nombre,
                Rol = rol
            };
        }

        public Task ClearSessionAsync()
        {
            SecureStorage.Remove(TokenKey);
            SecureStorage.Remove(NombreKey);
            SecureStorage.Remove(RolKey);
            return Task.CompletedTask;
        }
    }

    // Modelos auxiliares
    public class LoginResponse
    {
        public string Token { get; set; }
        public string Nombre { get; set; }
        public string Rol { get; set; }
    }

    public class UserSession
    {
        public string Token { get; set; }
        public string Nombre { get; set; }
        public string Rol { get; set; }
    }
}
