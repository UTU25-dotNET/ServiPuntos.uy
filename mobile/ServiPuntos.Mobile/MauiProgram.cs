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

            var apiBase = new Uri("https://ec2-18-220-251-96.us-east-2.compute.amazonaws.com:5019/");
            var authBase = new Uri(apiBase, "api/auth/");

            // AuthService + HttpClient para auth
            builder.Services
                .AddHttpClient<IAuthService, AuthService>(client =>
                {
                    client.BaseAddress = authBase;
                    client.DefaultRequestHeaders.Add("User-Agent", "ServiPuntos.Mobile");
                })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                });
            builder.Services.AddTransient<AuthMessageHandler>();

            // Inyección de AppShell
            builder.Services.AddSingleton<AppShell>();

            // Login
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<PushNotificationService>();
            builder.Services.AddTransient<RegisterPage>();

            // Tenant
            builder.Services
                .AddHttpClient<ITenantService, TenantService>(c => c.BaseAddress = apiBase)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                })
                .AddHttpMessageHandler<AuthMessageHandler>();

            // Points
            builder.Services
                .AddHttpClient<IPointsService, PointsService>(c => c.BaseAddress = apiBase)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                })
                .AddHttpMessageHandler<AuthMessageHandler>();
            builder.Services.AddTransient<PointsViewModel>();
            builder.Services.AddTransient<PointsPage>();

            // History
            builder.Services
                .AddHttpClient<IHistoryService, HistoryService>(c => c.BaseAddress = apiBase)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                })
                .AddHttpMessageHandler<AuthMessageHandler>();
            builder.Services.AddTransient<HistoryViewModel>();
            builder.Services.AddTransient<HistoryPage>();

            // Flash Offers
            builder.Services
                .AddHttpClient<IOfferService, OfferService>(c => c.BaseAddress = apiBase)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                })
                .AddHttpMessageHandler<AuthMessageHandler>();
            builder.Services.AddTransient<FlashOffersViewModel>();
            builder.Services.AddTransient<OffersPage>();

            // Alerts
            builder.Services
                .AddHttpClient<INotificationService, NotificationService>(c => c.BaseAddress = apiBase)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                })
                .AddHttpMessageHandler<AuthMessageHandler>();
            builder.Services.AddTransient<AlertsViewModel>();
            builder.Services.AddTransient<AlertsPage>();

            // Canje
            builder.Services
                .AddHttpClient<ICanjeService, CanjeService>(c => c.BaseAddress = apiBase)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                })
                .AddHttpMessageHandler<AuthMessageHandler>();

            // Product & Location (para catálogo necesitaremos LocationService)
            builder.Services
                .AddHttpClient<IProductService, ProductService>(c => c.BaseAddress = apiBase)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                })
                .AddHttpMessageHandler<AuthMessageHandler>();
            builder.Services
                .AddHttpClient<ILocationService, LocationService>(c => c.BaseAddress = apiBase)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                })
                .AddHttpMessageHandler<AuthMessageHandler>();

            // Usuarios (dev branch)
            builder.Services
                .AddHttpClient<IUserService, UserService>(c => c.BaseAddress = apiBase)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                })
                .AddHttpMessageHandler<AuthMessageHandler>();

            // Redemption, Profile, QR & Canjes (refacUI branch)
            builder.Services.AddTransient<RedemptionViewModel>();
            builder.Services.AddTransient<RedemptionPage>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddSingleton<QRCodePage>();
            builder.Services.AddTransient<CanjesViewModel>();
            builder.Services.AddTransient<CanjesPage>();

            // Catálogo
            builder.Services
                .AddHttpClient<ICatalogService, CatalogService>(c => c.BaseAddress = apiBase)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                })
                .AddHttpMessageHandler<AuthMessageHandler>();
            builder.Services.AddTransient<CatalogViewModel>();
            builder.Services.AddTransient<CatalogPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
