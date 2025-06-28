using ServiPuntos.Mobile.Services;
using ServiPuntos.Mobile.Views;
using static ServiPuntos.Mobile.Services.AppLogger;

namespace ServiPuntos.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            LogInfo("ServiPuntos Mobile iniciando...");
            LogInfo($"Timestamp: {DateTime.Now}");
            InitializeComponent();
            LogInfo("InitializeComponent completado");
            MainPage = new AppShell();
            LogInfo("AppShell asignado como MainPage");
        }

        // AGREGAR el manejo de deep links
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

                // Parsear los parámetros manualmente
                var queryString = uri.Query.TrimStart('?');
                var parameters = queryString.Split('&')
                    .Select(param => param.Split('='))
                    .Where(parts => parts.Length == 2)
                    .ToDictionary(parts => Uri.UnescapeDataString(parts[0]), 
                                 parts => Uri.UnescapeDataString(parts[1]));

                var token = parameters.ContainsKey("token") ? parameters["token"] : null;
                var error = parameters.ContainsKey("error") ? parameters["error"] : null;

                if (!string.IsNullOrEmpty(token))
                {
                    LogInfo($"[App] Token recibido en callback: {token.Substring(0, Math.Min(20, token.Length))}...");

                    await SecureStorage.SetAsync("auth_token", token);

                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        try
                        {
                            var authService = Handler?.MauiContext?.Services.GetService<IAuthService>();
                            if (authService != null)
                            {
                                var tokenDisplayPage = new TokenDisplayPage(authService);
                                tokenDisplayPage.SetToken(token);

                                LogInfo("[App] Navegando a TokenDisplayPage...");
                                await MainPage.Navigation.PushAsync(tokenDisplayPage);
                                LogInfo("[App] Navegacion completada");
                            }
                            else
                            {
                                LogInfo("[App] No se pudo obtener AuthService");
                                await MainPage.DisplayAlert("Token Recibido", 
                                    $"Token: {token.Substring(0, Math.Min(50, token.Length))}...", "OK");
                            }
                        }
                        catch (Exception navEx)
                        {
                            LogInfo($"[App] Error en navegacion: {navEx.Message}");
                            await MainPage.DisplayAlert("Token Recibido", 
                                $"Token JWT: {token.Substring(0, Math.Min(50, token.Length))}...", "OK");
                        }
                    });
                }
                else if (!string.IsNullOrEmpty(error))
                {
                    LogInfo($"[App] Error recibido en callback: {error}");
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await MainPage.DisplayAlert("Error de autenticacion", error, "OK");
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