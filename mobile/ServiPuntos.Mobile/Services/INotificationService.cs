using System.Collections.Generic;
using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public interface INotificationService
    {
        Task<List<NotificationDto>> GetMyNotificationsAsync();
        Task<bool> SetTokenFcmAsync(string token);

    }
}
