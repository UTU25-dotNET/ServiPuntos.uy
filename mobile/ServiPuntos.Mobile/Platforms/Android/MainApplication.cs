using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Firebase;
using Plugin.FirebasePushNotification;
using Android.Runtime;

namespace ServiPuntos.Mobile
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            // 1) Construye explícitamente un FirebaseOptions con los valores de tu google-services.json
            var options = new FirebaseOptions.Builder()
                .SetApplicationId("1:749916389089:android:361342f565c65cd64401a0")
                .SetApiKey("AIzaSyC6FPGjJqbnx7POxbc_P0p2g0pSBjm3UV0")
                .SetProjectId("servipuntos-7c094")
                .SetStorageBucket("servipuntos-7c094.firebasestorage.app")
                .SetDatabaseUrl("https://servipuntos-7c094.firebaseio.com")
                .Build();

            // 2) Inicializa el FirebaseApp ANTES del plugin
            try
            {
                FirebaseApp.InitializeApp(this, options);
                Log.Debug("ServiPuntos", "✔ FirebaseApp inicializado manualmente");
            }
            catch (Exception ex)
            {
                Log.Error("ServiPuntos", $"❌ Error inicializando FirebaseApp: {ex}");
            }

            // 3) Crea el canal para notificaciones en Android O+
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(
                    id: "default",
                    name: "General",
                    importance: NotificationImportance.High
                )
                {
                    Description = "Canal por defecto para ServiPuntos"
                };
                var manager = (NotificationManager)GetSystemService(NotificationService);
                manager.CreateNotificationChannel(channel);
            }

            // 4) Inicializa el plugin para token & “notification-only”
#if DEBUG
            FirebasePushNotificationManager.Initialize(this, true);
#else
            FirebasePushNotificationManager.Initialize(this, false);
#endif
        }

        protected override MauiApp CreateMauiApp() =>
            MauiProgram.CreateMauiApp();
    }
}
