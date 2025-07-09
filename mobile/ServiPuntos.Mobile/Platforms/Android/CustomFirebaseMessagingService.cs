using Android.App;
using Android.Util;
using Firebase.Messaging;
using AndroidX.Core.App;

namespace ServiPuntos.Mobile.Platforms.Android
{
    [Service(Name = "com.companyname.servipuntos.mobile.Platforms.Android.CustomFirebaseMessagingService")]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class CustomFirebaseMessagingService : FirebaseMessagingService
    {
        const string CHANNEL_ID = "default";

        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);

            // Loguea el contenido del mensaje
            Log.Debug("ServiPuntos-FG", $"[CustomFMS] MessageReceived data: {System.Text.Json.JsonSerializer.Serialize(message.Data)}");

            // Extrae título y cuerpo de forma segura
            var title = message.GetNotification()?.Title ?? (message.Data.ContainsKey("title") ? message.Data["title"] : string.Empty);
            var body = message.GetNotification()?.Body ?? (message.Data.ContainsKey("body") ? message.Data["body"] : string.Empty);

            // Construye la notificación
            var notif = new NotificationCompat.Builder(this, CHANNEL_ID)
                .SetContentTitle(title)
                .SetContentText(body)
                .SetSmallIcon(Resource.Mipmap.appicon)
                .SetAutoCancel(true)
                .Build();

            // Muestra la notificación
            NotificationManagerCompat.From(this)
                .Notify(new System.Random().Next(), notif);
        }

        public override void OnNewToken(string token)
        {
            base.OnNewToken(token);
            Log.Debug("ServiPuntos-FG", $"[CustomFMS] NewToken: {token}");
            // TODO: envar token al servidor al igual que hacemos en PushNotificationService…
        }
    }
}
