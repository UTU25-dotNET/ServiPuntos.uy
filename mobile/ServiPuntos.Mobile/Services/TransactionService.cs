using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly HttpClient _httpClient;
        public TransactionService(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<IEnumerable<TransactionDto>> GetUserTransactionsAsync(string userId)
        {
            var resp = await _httpClient.GetAsync($"usuario/{userId}");
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<IEnumerable<TransactionDto>>()
                   ?? Array.Empty<TransactionDto>();
        }
    }
}
