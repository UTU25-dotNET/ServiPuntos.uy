using System;
using System.Linq;
using ServiPuntos.Mobile.Services;
using static ServiPuntos.Mobile.Services.AppLogger;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;

namespace ServiPuntos.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            LogInfo("ServiPuntos Mobile iniciando...");
            LogInfo($"Timestamp: {DateTime.Now}");

            try
            {
                InitializeComponent();
                LogInfo("✔ InitializeComponent completado");
            }
            catch (Exception ex)
            {

                var realMsg = ex.InnerException?.Message ?? ex.Message;
                LogInfo($"❌ Error en InitializeComponent: {realMsg}");
                LogInfo(ex.ToString());  
                throw; 
            }

            MainPage = new AppShell();
            LogInfo("AppShell asignado como MainPage");
        }

        protected override void OnAppLinkRequestReceived(Uri uri)
        {
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
                    .ToDictionary(a => Uri.UnescapeDataString(a[0]),
                                  a => Uri.UnescapeDataString(a[1]));

                var token = parameters.GetValueOrDefault("token");
                var error = parameters.GetValueOrDefault("error");

                if (!string.IsNullOrEmpty(token))
                {
                    LogInfo($"[App] Token recibido: {token[..Math.Min(20, token.Length)]}...");
                    await SecureStorage.SetAsync("auth_token", token);

                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await MainPage.DisplayAlert(
                            "Autenticación exitosa",
                            "El token se guardó correctamente.",
                            "OK"
                        );
                    });
                }
                else if (!string.IsNullOrEmpty(error))
                {
                    LogInfo($"[App] Error recibido en callback: {error}");
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await MainPage.DisplayAlert(
                            "Error de autenticación",
                            error,
                            "OK"
                        );
                    });
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
}
