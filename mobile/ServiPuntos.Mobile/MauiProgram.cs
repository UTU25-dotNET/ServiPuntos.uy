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

            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<TokenDisplayPage>();

            builder.Services.AddTransient<AuthMessageHandler>();

            builder.Services
                .AddHttpClient<IPointsService, PointsService>(client =>
                {
                    client.BaseAddress = new Uri("https://ec2-18-220-251-96.us-east-2.compute.amazonaws.com:5019/");
                })
                .ConfigurePrimaryHttpMessageHandler(() =>
                    new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback =
                            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    })
                .AddHttpMessageHandler<AuthMessageHandler>();

            builder.Services.AddSingleton<PointsViewModel>();
            builder.Services.AddSingleton<PointsPage>();

            builder.Services
                .AddHttpClient<IHistoryService, HistoryService>(client =>
                {
                    client.BaseAddress = new Uri("https://ec2-18-220-251-96.us-east-2.compute.amazonaws.com:5019/");
                })
                .ConfigurePrimaryHttpMessageHandler(() =>
                    new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback =
                            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    })
                .AddHttpMessageHandler<AuthMessageHandler>();

            builder.Services.AddSingleton<HistoryViewModel>();
            builder.Services.AddSingleton<HistoryPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
