using System;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Maps;
using ServiPuntos.Mobile.Services;
using ServiPuntos.Mobile.ViewModels;
using ServiPuntos.Mobile.Views;
using static ServiPuntos.Mobile.Services.AppLogger;

namespace ServiPuntos.Mobile
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();

			builder
				.UseMauiApp<App>()
				.UseMauiMaps()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				});

#if DEBUG
            var apiBase = "http://10.0.2.2:5019/api/";
#else
			var apiBase = "https://ec2-18-220-251-96.us-east-2.compute.amazonaws.com:5019/api/";
#endif


			builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
			{
				client.BaseAddress = new Uri($"{apiBase}auth/");
				client.Timeout = TimeSpan.FromSeconds(60);
				client.DefaultRequestHeaders.Add("User-Agent", "ServiPuntos.Mobile");
			});


			builder.Services.AddHttpClient<IUbicacionService, UbicacionService>(client =>
			{
				client.BaseAddress = new Uri($"{apiBase}ubicacion/");
				client.Timeout = TimeSpan.FromSeconds(60);
				client.DefaultRequestHeaders.Add("User-Agent", "ServiPuntos.Mobile");
			});


			builder.Services.AddHttpClient<IProductoService, ProductoService>(client =>
			{
				client.BaseAddress = new Uri($"{apiBase}productoCanjeable/");
				client.Timeout = TimeSpan.FromSeconds(60);
				client.DefaultRequestHeaders.Add("User-Agent", "ServiPuntos.Mobile");
			});




			builder.Services.AddHttpClient<ICanjeService, CanjeService>(client =>
			{
				client.BaseAddress = new Uri($"{apiBase}nafta/");
				client.Timeout = TimeSpan.FromSeconds(60);
				client.DefaultRequestHeaders.Add("User-Agent", "ServiPuntos.Mobile");
			});

			builder.Services.AddHttpClient<IUserService, UserService>(client =>
			{
				client.BaseAddress = new Uri($"{apiBase}usuario/");
			});



			builder.Services.AddTransient<LoginViewModel>();
			builder.Services.AddTransient<HomeViewModel>();
			builder.Services.AddTransient<CanjeViewModel>();


			builder.Services.AddSingleton<TokenDisplayPage>();
			builder.Services.AddTransient<LoginPage>();
			builder.Services.AddTransient<HomePage>();
			builder.Services.AddTransient<CanjePage>();

#if DEBUG
            builder.Logging.AddDebug();
            builder.Logging.SetMinimumLevel(LogLevel.Debug);
#endif

			var app = builder.Build();


			var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
			Initialize(loggerFactory.CreateLogger("ServiPuntos"));
			LogInfo("🚀 ServiPuntos Mobile - Logger inicializado");
			LogDebug("🔧 Modo DEBUG activo");

			return app;
		}
	}
}
