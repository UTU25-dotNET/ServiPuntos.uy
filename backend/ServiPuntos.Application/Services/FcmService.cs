using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using ServiPuntos.Core.Interfaces;

namespace ServiPuntos.Application.Services
{
    public class FcmService : IFcmService
    {
        private readonly FirebaseMessaging _messaging;

        public FcmService()
        {
            // Usar la instancia ya inicializada en Program.cs
            if (FirebaseApp.DefaultInstance == null)
                throw new InvalidOperationException("Firebase app not initialized. Make sure Firebase is configured in Program.cs");
            
            _messaging = FirebaseMessaging.DefaultInstance;
        }

        public Task SendAsync(string token, string title, string body)
        {
            var message = new Message
            {
                Token = token,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                }
            };
            return _messaging.SendAsync(message);
        }
    }
}