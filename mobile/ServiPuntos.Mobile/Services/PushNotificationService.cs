using System;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using Plugin.FirebasePushNotifications;

namespace ServiPuntos.Mobile.Services
{
    public class PushNotificationService
    {
        const string TokenKey = "fcm_token";
        TaskCompletionSource<string?>? _tcs;
        bool _initialized;
        readonly IFirebasePushNotification _fcm;

        public PushNotificationService(IFirebasePushNotification fcm)
        {
            _fcm = fcm;
            Initialize();  // opcional: dentro del ctor, o llámalo explícito desde App.xaml.cs
        }

        // ← Cambiado de private void a public void
        public void Initialize()
        {
            if (_initialized) return;

            _fcm.TokenRefreshed += async (s, p) =>
            {
                if (!string.IsNullOrEmpty(p.Token))
                {
                    try
                    {
                        await SecureStorage.SetAsync(TokenKey, p.Token);
                        _tcs?.TrySetResult(p.Token);
                    }
                    catch (Exception ex)
                    {
                        AppLogger.LogInfo($"[PushNotificationService] Error storing token: {ex.Message}");
                    }
                }
            };

            _initialized = true;
        }

        public async Task<string?> RegisterAndRetrieveTokenAsync()
        {
            var token = _fcm.Token;
            if (!string.IsNullOrEmpty(token))
            {
                await SecureStorage.SetAsync(TokenKey, token);
                return token;
            }

            _tcs = new TaskCompletionSource<string?>();
            await _fcm.RegisterForPushNotificationsAsync();
            return await _tcs.Task;
        }

        public Task<string?> GetStoredTokenAsync() =>
            SecureStorage.GetAsync(TokenKey);
    }
}
