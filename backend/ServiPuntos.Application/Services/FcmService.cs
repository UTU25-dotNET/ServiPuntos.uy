using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;

namespace ServiPuntos.Application.Services
{
    public class FcmService
    {
        private readonly ILogger<FcmService> _logger;

        public FcmService(ILogger<FcmService> logger)
        {
            _logger = logger;
        }

        public async Task SendAsync(IEnumerable<string> tokens, string title, string body)
        {
            var list = tokens.Distinct().ToList();
            if (list.Count == 0) return;

            var message = new MulticastMessage
            {
                Tokens = list,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                }
            };
            try
            {
                await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending FCM notification");
            }
        }
    }
}
