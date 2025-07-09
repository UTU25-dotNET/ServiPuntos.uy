using Microsoft.Maui.Storage;
using Plugin.FirebasePushNotification;
using System;
using System.Threading.Tasks;

namespace ServiPuntos.Mobile.Services
{
    public class PushNotificationService
    {
        private const string TokenKey = "fcm_token";
        private TaskCompletionSource<string?>? _tcs;
        private bool _initialized;

        public void Initialize()
        {
            if (_initialized)
                return;

            CrossFirebasePushNotification.Current.OnTokenRefresh += async (s, p) =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(p.Token))
                    {
                        await SecureStorage.SetAsync(TokenKey, p.Token);
                        _tcs?.TrySetResult(p.Token);
                    }
                }
                catch (Exception ex)
                {
                    AppLogger.LogInfo($"[PushNotificationService] Error storing token: {ex.Message}");
                }
            };

            _initialized = true;
        }

        public async Task<string?> RegisterAndRetrieveTokenAsync()
        {
            Initialize();

            var token = CrossFirebasePushNotification.Current.Token;
            if (!string.IsNullOrEmpty(token))
            {
                await SecureStorage.SetAsync(TokenKey, token);
                return token;
            }

            _tcs = new TaskCompletionSource<string?>();
            CrossFirebasePushNotification.Current.RegisterForPushNotifications();
            return await _tcs.Task;
        }

        public Task<string?> GetStoredTokenAsync()
        {
            return SecureStorage.GetAsync(TokenKey);
        }
    }
}