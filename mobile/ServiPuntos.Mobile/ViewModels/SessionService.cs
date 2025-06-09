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
            await SecureStorage.Default.SetAsync(TokenKey, response.Token);
            await SecureStorage.Default.SetAsync(NombreKey, response.Nombre ?? "");
            await SecureStorage.Default.SetAsync(RolKey, response.Rol ?? "");
        }

        public async Task<UserSession> GetSessionAsync()
        {
            var token = await SecureStorage.Default.GetAsync(TokenKey);
            var nombre = await SecureStorage.Default.GetAsync(NombreKey);
            var rol = await SecureStorage.Default.GetAsync(RolKey);

            return new UserSession
            {
                Token = token,
                Nombre = nombre,
                Rol = rol
            };
        }

        public async Task ClearSessionAsync()
        {
            SecureStorage.Default.Remove(TokenKey);
            SecureStorage.Default.Remove(NombreKey);
            SecureStorage.Default.Remove(RolKey);
            await Task.CompletedTask;
        }
    }
}
