using Refit;
using Microsoft.Extensions.DependencyInjection;

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

            var apiBase = "https://10.0.2.2:5019/api/";

            // Handler para refrescar token
            builder.Services.AddTransient<RefreshTokenHandler>();

            // --- Auth ---
            builder.Services
                .AddRefitClient<IAuthApi, AuthService>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri($"{apiBase}auth/");
                    c.DefaultRequestHeaders.Add("User-Agent", "ServiPuntos.Mobile");
                })
                .AddHttpMessageHandler<RefreshTokenHandler>();

            // --- Usuario ---
            builder.Services
                .AddRefitClient<IUsuarioApi, UserService>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri($"{apiBase}usuario/");
                    c.DefaultRequestHeaders.Add("User-Agent", "ServiPuntos.Mobile");
                })
                .AddHttpMessageHandler<RefreshTokenHandler>();

            // --- VEAI ---
            builder.Services
                .AddRefitClient<IVeaiApi, VeaiService>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri($"{apiBase}verify/");
                    c.DefaultRequestHeaders.Add("User-Agent", "ServiPuntos.Mobile");
                })
                .AddHttpMessageHandler<RefreshTokenHandler>();

            // --- Ubicaciones ---
            builder.Services
                .AddRefitClient<IUbicacionApi, UbicacionService>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri($"{apiBase}ubicacion/");
                    c.DefaultRequestHeaders.Add("User-Agent", "ServiPuntos.Mobile");
                })
                .AddHttpMessageHandler<RefreshTokenHandler>();

            // --- Productos ---
            builder.Services
                .AddRefitClient<IProductoApi, ProductoService>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri($"{apiBase}productoUbicacion/");
                    c.DefaultRequestHeaders.Add("User-Agent", "ServiPuntos.Mobile");
                })
                .AddHttpMessageHandler<RefreshTokenHandler>();

            // --- Canjes ---
            builder.Services
                .AddRefitClient<ICanjeApi, CanjeService>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri($"{apiBase}nafta/");
                    c.DefaultRequestHeaders.Add("User-Agent", "ServiPuntos.Mobile");
                })
                .AddHttpMessageHandler<RefreshTokenHandler>();

            // --- Transacciones ---
            builder.Services
                .AddRefitClient<ITransactionApi, TransactionService>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri($"{apiBase}usuario/");
                    c.DefaultRequestHeaders.Add("User-Agent", "ServiPuntos.Mobile");
                })
                .AddHttpMessageHandler<RefreshTokenHandler>();

            // ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<CanjeViewModel>();
            builder.Services.AddTransient<MapaViewModel>();
            builder.Services.AddTransient<PerfilViewModel>();

            // Pages
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<CanjePage>();
            builder.Services.AddTransient<MapaPage>();
            builder.Services.AddTransient<PerfilPage>();

            // Logging
            builder.Logging.AddDebug();
            builder.Logging.SetMinimumLevel(LogLevel.Information);

            var app = builder.Build();

            // Inicializa logger estático
            var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
            Initialize(loggerFactory.CreateLogger("ServiPuntos"));
            LogInfo("🚀 ServiPuntos Mobile - Logger inicializado");
            LogDebug("🔧 Modo DEBUG activo");

            return app;
        }
    }
}
