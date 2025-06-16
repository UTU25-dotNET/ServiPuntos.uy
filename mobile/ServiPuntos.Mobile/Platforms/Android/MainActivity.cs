using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Content;
using static ServiPuntos.Mobile.Services.AppLogger;

namespace ServiPuntos.Mobile;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
[IntentFilter(new[] { Android.Content.Intent.ActionView },
    Categories = new[] { Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable },
    DataScheme = "servipuntos",
    DataHost = "auth-callback")]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        LogInfo("[MainActivity] OnCreate iniciado");
        base.OnCreate(savedInstanceState);
        
        // Manejar el intent inicial
        HandleIntent(Intent);
    }

    protected override void OnNewIntent(Intent? intent)
    {
        LogInfo("[MainActivity] OnNewIntent recibido");
        base.OnNewIntent(intent);
        
        // Manejar nuevos intents (cuando la app ya está corriendo)
        HandleIntent(intent);
    }

    private void HandleIntent(Intent? intent)
    {
        if (intent?.Data != null)
        {
            var uri = intent.Data.ToString();
            LogInfo($"[MainActivity] Intent con datos recibido: {uri}");
            
            // Convertir a Uri y notificar a la app
            try
            {
                var dotnetUri = new System.Uri(uri);
                LogInfo($"[MainActivity] Notificando a MAUI sobre deep link: {dotnetUri}");
                // In .NET MAUI, we handle app links through the App class or messaging center
                Microsoft.Maui.Controls.Application.Current?.SendOnAppLinkRequestReceived(dotnetUri);
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