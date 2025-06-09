using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiPuntos.Core.Interfaces;

namespace ServiPuntos.WebApp.Services
{
    public class AudienciaBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AudienciaBackgroundService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(30);

        public AudienciaBackgroundService(IServiceScopeFactory scopeFactory, ILogger<AudienciaBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var tenantService = scope.ServiceProvider.GetRequiredService<ITenantService>();
                    var audienciaService = scope.ServiceProvider.GetRequiredService<IAudienciaService>();

                    var tenants = await tenantService.GetAllAsync();
                    foreach (var tenant in tenants)
                    {
                        await audienciaService.ActualizarSegmentosUsuariosAsync(tenant.Id, null);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating audience segments in background");
                }

                try
                {
                    await Task.Delay(_interval, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    // ignored
                }
            }
        }
    }
}
