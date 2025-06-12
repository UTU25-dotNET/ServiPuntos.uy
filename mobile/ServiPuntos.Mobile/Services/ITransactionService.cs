using System.Collections.Generic;
using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionDto>> GetUserTransactionsAsync(string userId);
    }
}
