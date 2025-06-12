using Microsoft.Extensions.Logging;
using ServiPuntos.Mobile.Services;
using ServiPuntos.Mobile.Views;
using ServiPuntos.Mobile.ViewModels;
using static ServiPuntos.Mobile.Services.AppLogger;

namespace ServiPuntos.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        //HttpClient para AuthService
        builder.Services.AddHttpClient<AuthService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(120); // 
            client.DefaultRequestHeaders.Add("User-Agent", "ServiPuntos.Mobile");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .ConfigurePrimaryHttpMessageHandler(() => 
        {
            Console.WriteLine("[MauiProgram] Configurando HttpClientHandler.");
            var handler = new HttpClientHandler(); 
#if DEBUG
            // Este callback es para HTTPS. No se usa para llamadas HTTP al puerto 5020,
            // lo dejamos por ahora por si queremos volver a intentar usar HTTPS
            handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, certificate, chain, sslPolicyErrors) =>
            {
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                Console.WriteLine("[SSL VALIDATION CALLBACK - HttpClientHandler]");
                Console.WriteLine($"[SSL] Request URI: {httpRequestMessage.RequestUri}");
                Console.WriteLine($"[SSL] Cert Subject: {certificate?.Subject}");
                Console.WriteLine($"[SSL] SSL Policy Errors: {sslPolicyErrors}");
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                return true; // Aceptar todos los certificados en DEBUG para HTTPS
            };
#endif
            
            // Configuraciones generales para HttpClientHandler
            handler.UseCookies = false;
            handler.UseDefaultCredentials = false;
            handler.PreAuthenticate = false;
            handler.AllowAutoRedirect = false; // HttpClient maneja redirecciones por defecto, esto lo controla en el handler
            handler.MaxConnectionsPerServer = 10; // Opcional, valor por defecto suele ser suficiente
            
            Console.WriteLine($"[HttpClient] Handler configurado: {handler.GetType().Name}");
            
            return handler;
        });

        // Registrar servicios correctamente
        builder.Services.AddScoped<IAuthService, AuthService>();
        
        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        
        // Pages
        builder.Services.AddTransient<TokenDisplayPage>();
        builder.Services.AddTransient<LoginPage>();

#if DEBUG
        builder.Logging.AddDebug();
        builder.Logging.SetMinimumLevel(LogLevel.Debug);
#endif

        var app = builder.Build();
        
        // Llamo al logger
        var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("ServiPuntosApp"); //
        AppLogger.Initialize(logger); // Asegúrate que AppLogger.Initialize esté bien implementado
        
        // Log de inicio
        AppLogger.LogInfo("ServiPuntos Mobile - Sistema de logging inicializado");
        AppLogger.LogDebug("Modo DEBUG activo");

        return app;
    }
}