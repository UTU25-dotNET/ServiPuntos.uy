using System.Threading.Tasks;
using ServiPuntos.Core.DTOs;

namespace ServiPuntos.Core.Interfaces
{
    public interface IPayPalService
    {
        Task<TransaccionPayPalDto> CreatePaymentAsync(decimal amount, string currency = "USD", string description = "ServiPuntos Transaction");
        Task<TransaccionPayPalDto> ExecutePaymentAsync(string paymentId, string payerId);
        Task<TransaccionPayPalDto> GetPaymentDetailsAsync(string paymentId);
        Task<bool> ValidatePaymentAsync(string paymentId, decimal expectedAmount);
    }
}