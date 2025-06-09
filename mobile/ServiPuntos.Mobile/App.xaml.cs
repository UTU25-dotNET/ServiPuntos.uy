using System;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using static ServiPuntos.Mobile.Services.AppLogger;

namespace ServiPuntos.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            LogInfo("ServiPuntos Mobile iniciado");
            MainPage = new AppShell();
        }

        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            LogInfo($"Deep link recibido: {uri}");
            if (uri.Scheme == "servipuntos" && uri.Host == "auth-callback")
                HandleAuthCallback(uri);
            else
                LogDebug($"Deep link no reconocido: {uri}");

            base.OnAppLinkRequestReceived(uri);
        }

        private async void HandleAuthCallback(Uri uri)
        {
            try
            {
                LogInfo($"Procesando callback: {uri.Query}");
                var query = uri.Query
                               .TrimStart('?')
                               .Split('&', StringSplitOptions.RemoveEmptyEntries)
                               .Select(p => p.Split('='))
                               .ToDictionary(kv => Uri.UnescapeDataString(kv[0]),
                                             kv => Uri.UnescapeDataString(kv[1]));

                if (query.TryGetValue("token", out var token))
                {
                    LogInfo($"Token recibido: {token[..Math.Min(20, token.Length)]}...");
                    await SecureStorage.SetAsync("auth_token", token);
                    await MainPage.Navigation.PushAsync(new Views.TokenDisplayPage(token));
                }
                else if (query.TryGetValue("error", out var error))
                {
                    LogInfo($"Error en callback: {error}");
                    await MainPage.DisplayAlert("Error autenticación", error, "OK");
                }
            }
            catch (Exception ex)
            {
                LogInfo($"Error manejando callback: {ex.Message}");
            }
        }
    }
}
