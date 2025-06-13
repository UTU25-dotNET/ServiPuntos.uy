using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<CanjeHistorialItem>> GetUserTransactionsAsync(string userId);
    }

    public class TransactionService : ITransactionService
    {
        private readonly HttpClient _httpClient;

        public TransactionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<IEnumerable<CanjeHistorialItem>> GetUserTransactionsAsync(string userId)
        {
            var resp = await _httpClient.GetAsync($"usuario/{userId}");
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<IEnumerable<CanjeHistorialItem>>() ?? new List<CanjeHistorialItem>();
        }
    }
}
