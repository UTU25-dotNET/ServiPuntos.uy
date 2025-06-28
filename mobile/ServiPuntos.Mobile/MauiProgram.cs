using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using ServiPuntos.Mobile.Handlers;
using ServiPuntos.Mobile.Services;
using ServiPuntos.Mobile.ViewModels;
using ServiPuntos.Mobile.Views;

namespace ServiPuntos.Mobile
{
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

            // --- Registro de autenticación ---
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<TokenDisplayPage>();
            builder.Services.AddTransient<AuthMessageHandler>();

            var apiBase = new Uri("https://ec2-18-220-251-96.us-east-2.compute.amazonaws.com:5019/");

            // --- Servicios y vistas puntaje ---
            builder.Services
                .AddHttpClient<IPointsService, PointsService>(c => c.BaseAddress = apiBase)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                })
                .AddHttpMessageHandler<AuthMessageHandler>();
            builder.Services.AddSingleton<PointsViewModel>();
            builder.Services.AddSingleton<PointsPage>();

            // --- Servicios e historial ---
            builder.Services
                .AddHttpClient<IHistoryService, HistoryService>(c => c.BaseAddress = apiBase)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                })
                .AddHttpMessageHandler<AuthMessageHandler>();
            builder.Services.AddSingleton<HistoryViewModel>();
            builder.Services.AddSingleton<HistoryPage>();

            // --- Ofertas flash ---
            builder.Services
                .AddHttpClient<IOfferService, OfferService>(c => c.BaseAddress = apiBase)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                })
                .AddHttpMessageHandler<AuthMessageHandler>();
            builder.Services.AddSingleton<FlashOffersViewModel>();
            builder.Services.AddSingleton<OffersPage>();

            // --- Alertas ---
            builder.Services
                .AddHttpClient<INotificationService, NotificationService>(c => c.BaseAddress = apiBase)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                })
                .AddHttpMessageHandler<AuthMessageHandler>();
            builder.Services.AddSingleton<AlertsViewModel>();
            builder.Services.AddSingleton<AlertsPage>();

            // --- Canje / Redemption ---
            builder.Services
                .AddHttpClient<ICanjeService, CanjeService>(c => c.BaseAddress = apiBase)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                })
                .AddHttpMessageHandler<AuthMessageHandler>();
            builder.Services.AddSingleton<RedemptionViewModel>();
            builder.Services.AddSingleton<RedemptionPage>();

            // --- Página de QR Code (usando QRCoder en el code-behind) ---
            builder.Services.AddSingleton<QRCodePage>();

            // --- Productos ---
            builder.Services
                .AddHttpClient<IProductService, ProductService>(c => c.BaseAddress = apiBase)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                })
                .AddHttpMessageHandler<AuthMessageHandler>();

            // --- Ubicaciones ---
            builder.Services
                .AddHttpClient<ILocationService, LocationService>(c => c.BaseAddress = apiBase)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                })
                .AddHttpMessageHandler<AuthMessageHandler>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
