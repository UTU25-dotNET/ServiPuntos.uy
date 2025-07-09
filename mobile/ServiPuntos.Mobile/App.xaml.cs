using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AndroidX.Core.App;
using Android.Util;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Plugin.FirebasePushNotification;
using ServiPuntos.Mobile.Services;
using static ServiPuntos.Mobile.Services.AppLogger;

namespace ServiPuntos.Mobile;

public partial class App : Microsoft.Maui.Controls.Application
{
    private readonly PushNotificationService _pushService;
    private readonly INotificationService _notificationService;

    public App(PushNotificationService pushService,
               INotificationService notificationService)
    {
        Log.Debug("ServiPuntos-App", "App constructor - iniciando");

        InitializeComponent();

        // Early hook para notificaciones antes de que la UI esté lista
        CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
        {
            var payload = JsonSerializer.Serialize(p.Data);
            Log.Debug("ServiPuntos-App", $"[App] Early Received: {payload}");
            LogInfo($"[App] Early Received: {payload}");
        };

        _pushService = pushService;
        _notificationService = notificationService;

        LogInfo("ServiPuntos Mobile iniciando...");
        LogInfo($"Timestamp: {DateTime.Now}");

        try
        {
            Log.Debug("ServiPuntos-App", "InitializeComponent completado");
            LogInfo("✔ InitializeComponent completado");

            _pushService.Initialize();
            Log.Debug("ServiPuntos-App", "PushNotificationService.Initialize() llamado");

            // --- Aquí inyectamos la notificación también en foreground ---
            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {
                // Extraemos título y cuerpo de p.Data de forma segura
                string title = "";
                string body = "";
                if (p.Data.TryGetValue("title", out var t) && t != null)
                    title = t.ToString();
                if (p.Data.TryGetValue("body", out var b) && b != null)
                    body = b.ToString();

                Log.Debug("ServiPuntos-Push-FG", $"FG Received ➜ {title} | {body}");

                // Construye y muestra una notificación local
                var notif = new NotificationCompat.Builder(
                                 Android.App.Application.Context,
                                 "default")
                    .SetContentTitle(title)
                    .SetContentText(body)
                    .SetSmallIcon(Resource.Mipmap.appicon)
                    .SetAutoCancel(true)
                    .Build();

                NotificationManagerCompat
                    .From(Android.App.Application.Context)
                    .Notify(new Random().Next(), notif);
            };

            // Logs de los demás callbacks
            CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
                Log.Debug("ServiPuntos-App", $"[App] OnTokenRefresh: {p.Token}");

            CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
            {
                var d = JsonSerializer.Serialize(p.Data);
                Log.Debug("ServiPuntos-App", $"[App] OnNotificationOpened: {d}");
                LogInfo($"[App] OnNotificationOpened: {d}");
            };
            CrossFirebasePushNotification.Current.OnNotificationDeleted += (s, p) =>
            {
                Log.Debug("ServiPuntos-App", "[App] OnNotificationDeleted");
                LogInfo("[App] OnNotificationDeleted");
            };
        }
        catch (Exception ex)
        {
            var realMsg = ex.InnerException?.Message ?? ex.Message;
            Log.Debug("ServiPuntos-App", $"Error en InitializeComponent: {realMsg}");
            LogInfo($"❌ Error en InitializeComponent: {realMsg}");
            LogInfo(ex.ToString());
            throw;
        }

        MainPage = new AppShell();
        Log.Debug("ServiPuntos-App", "AppShell asignado como MainPage");
        LogInfo("AppShell asignado como MainPage");

        // Inicia flujo de registro de token FCM
        _ = RegisterPushTokenAsync();
    }

    private async Task RegisterPushTokenAsync()
    {
        Log.Debug("ServiPuntos-App", "RegisterPushTokenAsync() - comenzando");
        try
        {
            var token = await _pushService.RegisterAndRetrieveTokenAsync();
            Log.Debug("ServiPuntos-App", $"Token obtenido: {token}");

            if (!string.IsNullOrEmpty(token))
            {
                var success = await _notificationService.SetTokenFcmAsync(token);
                if (success)
                {
                    Log.Debug("ServiPuntos-App", "Token FCM registrado correctamente");
                    LogInfo($"[App] Token FCM registrado correctamente: {token}");
                }
                else
                {
                    Log.Debug("ServiPuntos-App", "Falló el registro del token FCM");
                    LogInfo("[App] Falló el registro del token FCM");
                }
            }
            else
            {
                Log.Debug("ServiPuntos-App", "No se obtuvo token FCM");
            }
        }
        catch (Exception ex)
        {
            Log.Debug("ServiPuntos-App", $"Error registrando token FCM: {ex}");
            LogInfo($"[App] Error registrando token FCM: {ex.Message}");
        }
    }

    protected override void OnAppLinkRequestReceived(Uri uri)
    {
        Log.Debug("ServiPuntos-App", $"OnAppLinkRequestReceived: {uri}");
        LogInfo($"[App] Deep link recibido: {uri}");
        LogInfo($"[App] Scheme: {uri.Scheme}");
        LogInfo($"[App] Host: {uri.Host}");
        LogInfo($"[App] Query: {uri.Query}");

        if (uri.Scheme == "servipuntos" && uri.Host == "auth-callback")
        {
            LogInfo("[App] Callback de autenticacion detectado");
            HandleAuthCallback(uri);
        }
        else
        {
            LogInfo($"[App] Deep link no reconocido: {uri}");
        }

        base.OnAppLinkRequestReceived(uri);
    }

    private async void HandleAuthCallback(Uri uri)
    {
        try
        {
            LogInfo($"[App] Procesando callback: {uri.Query}");

            var qs = uri.Query.TrimStart('?');
            var parameters = qs
                .Split('&')
                .Select(p => p.Split('='))
                .Where(a => a.Length == 2)
                .ToDictionary(
                    a => Uri.UnescapeDataString(a[0]),
                    a => Uri.UnescapeDataString(a[1]));

            parameters.TryGetValue("token", out var token);
            parameters.TryGetValue("error", out var error);

            if (!string.IsNullOrEmpty(token))
            {
                LogInfo($"[App] Token recibido: {token[..Math.Min(20, token.Length)]}...");
                await SecureStorage.SetAsync("auth_token", token);
                await MainPage.DisplayAlert(
                    "Autenticación exitosa",
                    "El token se guardó correctamente.",
                    "OK");
            }
            else if (!string.IsNullOrEmpty(error))
            {
                LogInfo($"[App] Error recibido en callback: {error}");
                await MainPage.DisplayAlert("Error de autenticación", error, "OK");
            }
            else
            {
                LogInfo("[App] Callback sin token ni error");
            }
        }
        catch (Exception ex)
        {
            LogInfo($"[App] Error procesando callback: {ex.Message}");
        }
    }
}
