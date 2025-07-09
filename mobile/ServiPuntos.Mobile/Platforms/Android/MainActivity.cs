using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using static ServiPuntos.Mobile.Services.AppLogger;

// ← Este using es el que faltaba:
using Microsoft.Maui.Controls;

namespace ServiPuntos.Mobile
{
    [Activity(
        Theme = "@style/Maui.SplashTheme",
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges = ConfigChanges.ScreenSize
                             | ConfigChanges.Orientation
                             | ConfigChanges.UiMode
                             | ConfigChanges.ScreenLayout
                             | ConfigChanges.SmallestScreenSize
                             | ConfigChanges.Density)]
    [IntentFilter(new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "servipuntos",
        DataHost = "auth-callback")]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            LogInfo("[MainActivity] OnCreate iniciado");
            base.OnCreate(savedInstanceState);
            HandleIntent(Intent);
        }

        protected override void OnNewIntent(Intent? intent)
        {
            LogInfo("[MainActivity] OnNewIntent recibido");
            base.OnNewIntent(intent);
            HandleIntent(intent);
        }

        void HandleIntent(Intent? intent)
        {
            if (intent?.Data != null)
            {
                var uri = intent.Data.ToString();
                LogInfo($"[MainActivity] Intent con datos recibido: {uri}");

                try
                {
                    var dotnetUri = new Uri(uri);
                    LogInfo($"[MainActivity] Notificando a MAUI sobre deep link: {dotnetUri}");

                    // Aquí usamos el tipo completo para evitar la colisión
                    Microsoft.Maui.Controls.Application.Current?
                        .SendOnAppLinkRequestReceived(dotnetUri);
                }
                catch (Exception ex)
                {
                    LogInfo($"[MainActivity] Error procesando intent: {ex.Message}");
                }
            }
            else
            {
                LogInfo("[MainActivity] Intent sin datos recibido");
            }
        }

    }
}
