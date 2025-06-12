// TransactionService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    // Refit API interface
    public interface ITransactionApi
    {
        // GET api/usuario/historial-transacciones
        [Get("/historial-transacciones")]
        Task<IEnumerable<TransactionDto>> GetTransactionsAsync();
    }

    // Service interface
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionDto>> GetUserTransactionsAsync();
    }

    // Service implementation using Refit
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionApi _api;

        public TransactionService(ITransactionApi api)
        {
            _api = api;
        }

        public async Task<IEnumerable<TransactionDto>> GetUserTransactionsAsync()
        {
            try
            {
                var list = await _api.GetTransactionsAsync();
                return list ?? Array.Empty<TransactionDto>();
            }
            catch (ApiException ex)
            {
                throw new InvalidOperationException($"Error al obtener transacciones: {ex.StatusCode}", ex);
            }
        }
    }
}
