using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Refit;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    // Refit API interface para UsuarioController
    public interface IUsuarioApi
    {
        // GET api/usuario/email/{email}
        [Get("/email/{email}")]
        Task<Usuario> GetByEmailAsync(string email);

        // GET api/usuario/{id}
        [Get("/{id}")]
        Task<Usuario> GetByIdAsync(string id);

        // PUT api/usuario/{id}
        [Put("/{id}")]
        Task<ApiResponse<object>> UpdateAsync(string id, [Body] Usuario usuario);

        // GET api/usuario/historial-transacciones
        [Get("/historial-transacciones")]
        Task<List<TransactionSummary>> GetHistoryAsync();

        // ** NUEVO ** GET api/usuario/balance
        [Get("/balance")]
        Task<int> GetBalanceAsync();
    }

    // Servicio de alto nivel para el cliente MAUI
    public interface IUserService
    {
        Task<Usuario?> GetPerfilByEmailAsync(string email);
        Task<Usuario?> GetPerfilByIdAsync(string id);
        Task<bool> UpdatePerfilAsync(Usuario usuario);
        Task<List<TransactionSummary>?> GetRecentTransactionsAsync();

        // ** NUEVO ** expuesto para HomeViewModel
        Task<int> GetBalanceAsync();
    }

    public class UserService : IUserService
    {
        private readonly IUsuarioApi _api;

        public UserService(IUsuarioApi api)
        {
            _api = api;
        }

        public async Task<Usuario?> GetPerfilByEmailAsync(string email)
        {
            try
            {
                return await _api.GetByEmailAsync(email);
            }
            catch (ApiException)
            {
                return null;
            }
        }

        public async Task<Usuario?> GetPerfilByIdAsync(string id)
        {
            try
            {
                return await _api.GetByIdAsync(id);
            }
            catch (ApiException)
            {
                return null;
            }
        }

        public async Task<bool> UpdatePerfilAsync(Usuario usuario)
        {
            try
            {
                var response = await _api.UpdateAsync(usuario.Id.ToString(), usuario);
                return response.IsSuccessStatusCode;
            }
            catch (ApiException)
            {
                return false;
            }
        }

        public async Task<List<TransactionSummary>?> GetRecentTransactionsAsync()
        {
            try
            {
                var all = await _api.GetHistoryAsync();
                // Devuelve sólo las 5 transacciones más recientes
                return all.OrderByDescending(t => t.Fecha).Take(5).ToList();
            }
            catch (ApiException)
            {
                return null;
            }
        }

        // **Implementación del balance**
        public async Task<int> GetBalanceAsync()
        {
            try
            {
                return await _api.GetBalanceAsync();
            }
            catch (ApiException)
            {
                // En caso de error devolvemos 0 o podrías lanzar
                return 0;
            }
        }
    }
}
