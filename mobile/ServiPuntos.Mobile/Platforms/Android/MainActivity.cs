using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using static ServiPuntos.Mobile.Services.AppLogger;
using Plugin.FirebasePushNotification;

namespace ServiPuntos.Mobile;

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
    const int RequestNotificationPermissionId = 1001;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        LogInfo("[MainActivity] OnCreate iniciado");
        base.OnCreate(savedInstanceState);

        // Procesa intents de notificación
        FirebasePushNotificationManager.ProcessIntent(this, Intent);

        // Solicita permiso de notificaciones en Android 13+
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
        {
            if (CheckSelfPermission(Android.Manifest.Permission.PostNotifications)
                != Permission.Granted)
            {
                RequestPermissions(
                    new[] { Android.Manifest.Permission.PostNotifications },
                    RequestNotificationPermissionId);
            }
        }

        HandleIntent(Intent);
    }

    protected override void OnNewIntent(Intent? intent)
    {
        LogInfo("[MainActivity] OnNewIntent recibido");
        base.OnNewIntent(intent);
        FirebasePushNotificationManager.ProcessIntent(this, intent);
        HandleIntent(intent);
    }

    public override void OnRequestPermissionsResult(
        int requestCode, string[] permissions, Permission[] grantResults)
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        if (requestCode == RequestNotificationPermissionId)
        {
            var granted = grantResults.Length > 0
                          && grantResults[0] == Permission.Granted;
            LogInfo($"[MainActivity] POST_NOTIFICATIONS permission granted: {granted}");
        }
    }

    void HandleIntent(Intent? intent)
    {
        if (intent?.Data != null)
        {
            var uri = intent.Data.ToString();
            LogInfo($"[MainActivity] Intent con datos recibido: {uri}");
            try
            {
                var dotnetUri = new System.Uri(uri);
                LogInfo($"[MainActivity] Notificando a MAUI sobre deep link: {dotnetUri}");
                Microsoft.Maui.Controls.Application.Current
                    ?.SendOnAppLinkRequestReceived(dotnetUri);
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
