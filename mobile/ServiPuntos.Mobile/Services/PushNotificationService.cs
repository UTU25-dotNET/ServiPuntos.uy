using System;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using Plugin.FirebasePushNotification;
using static ServiPuntos.Mobile.Services.AppLogger;
using Android.Util;

namespace ServiPuntos.Mobile.Services
{
    public class PushNotificationService
    {
        private const string TokenKey = "fcm_token";
        private readonly INotificationService _notificationService;
        private TaskCompletionSource<string?>? _tcs;
        private bool _initialized;

        public PushNotificationService(INotificationService notificationService)
        {
            _notificationService = notificationService;
            Log.Debug("ServiPuntos-Push", "Constructor PushNotificationService()");
        }

        public void Initialize()
        {
            if (_initialized)
            {
                Log.Debug("ServiPuntos-Push", "Initialize() ya fue llamado, ignorando.");
                return;
            }

            Log.Debug("ServiPuntos-Push", "Initialize() - registrando eventos de FCM");

            // Cuando el token es refrescado o generado por primera vez
            CrossFirebasePushNotification.Current.OnTokenRefresh += async (s, p) =>
            {
                Log.Debug("ServiPuntos-Push", $"OnTokenRefresh triggered: token={p.Token}");
                try
                {
                    if (!string.IsNullOrEmpty(p.Token))
                    {
                        await SecureStorage.SetAsync(TokenKey, p.Token);
                        Log.Debug("ServiPuntos-Push", "Token guardado en SecureStorage");

                        var success = await _notificationService.SetTokenFcmAsync(p.Token);
                        Log.Debug("ServiPuntos-Push", $"Token enviado al backend: success={success}");

                        _tcs?.TrySetResult(p.Token);
                        LogInfo($"[PushNotificationService] Token registrado y enviado al backend: {p.Token}");
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug("ServiPuntos-Push", $"Error en OnTokenRefresh: {ex}");
                    LogInfo($"[PushNotificationService] Error al almacenar o enviar token: {ex.Message}");
                }
            };

            // Notificaciones recibidas en foreground o background
            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {
                var dataJson = System.Text.Json.JsonSerializer.Serialize(p.Data);
                Log.Debug("ServiPuntos-Push", $"OnNotificationReceived: data={dataJson}");
                LogInfo($"[PushNotificationService] Notificación recibida: {dataJson}");
            };

            // Usuario clicó la notificación
            CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
            {
                var dataJson = System.Text.Json.JsonSerializer.Serialize(p.Data);
                Log.Debug("ServiPuntos-Push", $"OnNotificationOpened: data={dataJson}");
                LogInfo($"[PushNotificationService] Notificación abierta: {dataJson}");
            };

            // Sistema descartó la notificación (swipe away)
            CrossFirebasePushNotification.Current.OnNotificationDeleted += (s, p) =>
            {
                Log.Debug("ServiPuntos-Push", "OnNotificationDeleted");
                LogInfo("[PushNotificationService] Notificación descartada por sistema");
            };

            _initialized = true;
            Log.Debug("ServiPuntos-Push", "Initialize() completo");
        }

        public async Task<string?> RegisterAndRetrieveTokenAsync()
        {
            Log.Debug("ServiPuntos-Push", "RegisterAndRetrieveTokenAsync() - comenzando");
            Initialize();

            var token = CrossFirebasePushNotification.Current.Token;
            if (!string.IsNullOrEmpty(token))
            {
                Log.Debug("ServiPuntos-Push", $"Token existente detectado: {token}");
                await SecureStorage.SetAsync(TokenKey, token);

                try
                {
                    var success = await _notificationService.SetTokenFcmAsync(token);
                    Log.Debug("ServiPuntos-Push", $"Token existente enviado al backend: success={success}");
                    LogInfo($"[PushNotificationService] Token existente enviado al backend: {token}");
                }
                catch (Exception ex)
                {
                    Log.Debug("ServiPuntos-Push", $"Error enviando token existente: {ex}");
                    LogInfo($"[PushNotificationService] Error al enviar token existente: {ex.Message}");
                }

                return token;
            }

            Log.Debug("ServiPuntos-Push", "No había token: registrando para push notifications");
            _tcs = new TaskCompletionSource<string?>();
            CrossFirebasePushNotification.Current.RegisterForPushNotifications();
            return await _tcs.Task;
        }

        public Task<string?> GetStoredTokenAsync()
        {
            Log.Debug("ServiPuntos-Push", "GetStoredTokenAsync()");
            return SecureStorage.GetAsync(TokenKey);
        }
    }
}
