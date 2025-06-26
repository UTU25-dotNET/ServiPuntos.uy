using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using ServiPuntos.Mobile.Handlers;
using ServiPuntos.Mobile.Services;
using ServiPuntos.Mobile.ViewModels;
using ServiPuntos.Mobile.Views;
using System;

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

            var apiBase = new Uri("https://ec2-18-220-251-96.us-east-2.compute.amazonaws.com:5019/");

            // Auth
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<TokenDisplayPage>();
            builder.Services.AddTransient<AuthMessageHandler>();

            // Points
            builder.Services
                .AddHttpClient<IPointsService, PointsService>(c => c.BaseAddress = apiBase)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                })
                .AddHttpMessageHandler<AuthMessageHandler>();
            builder.Services.AddSingleton<PointsViewModel>();
            builder.Services.AddSingleton<PointsPage>();

            // History
            builder.Services
                .AddHttpClient<IHistoryService, HistoryService>(c => c.BaseAddress = apiBase)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                })
                .AddHttpMessageHandler<AuthMessageHandler>();
            builder.Services.AddSingleton<HistoryViewModel>();
            builder.Services.AddSingleton<HistoryPage>();

            // Offers
            builder.Services
                .AddHttpClient<IOfferService, OfferService>(c => c.BaseAddress = apiBase)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                })
                .AddHttpMessageHandler<AuthMessageHandler>();
            builder.Services.AddSingleton<FlashOffersViewModel>();
            builder.Services.AddSingleton<OffersPage>();

            // Alerts
            builder.Services
                .AddHttpClient<INotificationService, NotificationService>(c => c.BaseAddress = apiBase)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                })
                .AddHttpMessageHandler<AuthMessageHandler>();
            builder.Services.AddSingleton<AlertsViewModel>();
            builder.Services.AddSingleton<AlertsPage>();

            // Redemption
            builder.Services
                .AddHttpClient<ICanjeService, CanjeService>(c => c.BaseAddress = apiBase)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                })
                .AddHttpMessageHandler<AuthMessageHandler>();
            builder.Services.AddSingleton<RedemptionViewModel>();
            builder.Services.AddSingleton<RedemptionPage>();

            // QR Code
            builder.Services.AddSingleton<QRCodePage>();

            // Locations
            builder.Services
                .AddHttpClient<ILocationService, LocationService>(c => c.BaseAddress = apiBase)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                })
                .AddHttpMessageHandler<AuthMessageHandler>();
            builder.Services.AddTransient<LocationsViewModel>();
            builder.Services.AddTransient<LocationsPage>();

            // Catalog
            builder.Services
                .AddHttpClient<IProductService, ProductService>(c => c.BaseAddress = apiBase)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                })
                .AddHttpMessageHandler<AuthMessageHandler>();
            builder.Services.AddTransient<CatalogViewModel>();
            builder.Services.AddTransient<CatalogPage>();

            // Map (WebView-based)
            builder.Services.AddTransient<MapViewModel>();
            builder.Services.AddTransient<MapPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
