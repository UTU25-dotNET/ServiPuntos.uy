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

        public FcmService(IConfiguration configuration)
        {
            var path = configuration["Firebase:CredentialsPath"];
            if (string.IsNullOrEmpty(path))
                throw new InvalidOperationException("Firebase credentials not configured");

            if (FirebaseApp.DefaultInstance == null)
            {
                var credential = GoogleCredential
                    .FromFile(path)
                    .CreateScoped(new[]
				  {
				    "https://www.googleapis.com/auth/firebase.messaging",
				    "https://www.googleapis.com/auth/cloud-platform"
				  });
                FirebaseApp.Create(new AppOptions
                {
                    Credential = credential
                });
            }
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
