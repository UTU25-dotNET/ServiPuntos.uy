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

			builder.Services.AddTransient<LoginViewModel>();
			builder.Services.AddHttpClient<IAuthService, AuthService>();
        	builder.Services.AddSingleton<IAuthService, AuthService>();

			//builder.Services.AddTransient<TenantSelectorViewModel>();
			builder.Services.AddTransient<TokenDisplayPage>();
        	builder.Services.AddTransient<LoginPage>();

#if DEBUG
		builder.Logging.AddDebug();
		builder.Logging.SetMinimumLevel(LogLevel.Debug);
#endif

		var app = builder.Build();
		
		// Inicializar nuestro logger personalizado
		var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
		var logger = loggerFactory.CreateLogger("ServiPuntos");
		AppLogger.Initialize(logger);
		
		// Log de inicio
		AppLogger.LogInfo("🚀 ServiPuntos Mobile - Sistema de logging inicializado");
		AppLogger.LogDebug("🔧 Modo DEBUG activo");

		return app;
	}
}



