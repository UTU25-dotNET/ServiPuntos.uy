using Microsoft.AspNetCore.Mvc.Filters;
using ServiPuntos.Core.Interfaces;

namespace ServiPuntos.WebApp.Filters
{
    public class TenantNavbarFilter : IAsyncActionFilter
    {
        private readonly ITenantService _tenantService;

        public TenantNavbarFilter(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Setear el nombre del tenant para el navbar
            await SetTenantNameForNavbar(context);
            
            // Continuar con la ejecución normal
            await next();
        }

        private async Task SetTenantNameForNavbar(ActionExecutingContext context)
        {
            var controller = context.Controller as Microsoft.AspNetCore.Mvc.Controller;
            if (controller == null) return;

            // Valores por defecto
            controller.ViewBag.AppName = "ServiPuntos.WebApp";
            controller.ViewBag.TenantColor = null; // Sin color personalizado
            controller.ViewBag.TenantLogo = "/images/placeholder-logo.png";

            if (context.HttpContext.User.IsInRole("AdminTenant"))
            {
                try
                {
                    var tenantIdClaim = context.HttpContext.User.Claims
                        .FirstOrDefault(c => c.Type == "tenantId")?.Value;
                        
                    if (!string.IsNullOrEmpty(tenantIdClaim) && Guid.TryParse(tenantIdClaim, out Guid tenantId))
                    {
                        var tenant = await _tenantService.GetByIdAsync(tenantId);
                        if (tenant != null)
                        {
                            controller.ViewBag.AppName = tenant.Nombre;
                            
                            // Aplicar color del tenant si existe
                            if (!string.IsNullOrEmpty(tenant.Color))
                            {
                                controller.ViewBag.TenantColor = tenant.Color;
                            }

                            // Logo del tenant (o placeholder si no existe)
                            if (!string.IsNullOrEmpty(tenant.LogoUrl))
                            {
                                controller.ViewBag.TenantLogo = tenant.LogoUrl;
                            }
                        }
                        else
                        {
                            controller.ViewBag.AppName = "ServiPuntos - Tenant";
                        }
                    }
                }
                catch (Exception ex)
                {
                    // En caso de error, usar fallback
                    Console.WriteLine($"Error al obtener tenant para navbar: {ex.Message}");
                    controller.ViewBag.AppName = "ServiPuntos - Tenant";
                }
            }
        }
    }
}