using System.Collections.Generic;
using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public interface IUserService
    {
        Task<Usuario?> GetPerfilByEmailAsync(string email);
        Task<Usuario?> GetPerfilByIdAsync(string id);
        Task<bool> UpdatePerfilAsync(Usuario usuario);
        Task<PointBalanceResponse?> GetBalanceAsync();
        Task<List<TransactionSummary>?> GetRecentTransactionsAsync();
    }
}
