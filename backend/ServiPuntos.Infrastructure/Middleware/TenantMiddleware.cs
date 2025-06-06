﻿using Microsoft.AspNetCore.Http;

namespace ServiPuntos.Infrastructure.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ITenantResolver resolver, ITenantContext tenantContext)
        {
            Guid tenantId;

            // 1. Si está autenticado (backoffice), usar el claim
            var tenantClaim = context.User?.FindFirst("tenantId");
            if (tenantClaim != null && Guid.TryParse(tenantClaim.Value, out tenantId))
            {
                tenantContext.TenantId = tenantId;
                await _next(context);
                return;
            }

            // 2. Si es una request API, buscar el header
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                try
                {
                    tenantContext.TenantId = resolver.GetCurrentTenantId();
                }
                catch
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("TenantId inválido o ausente.");
                    return;
                }

                await _next(context);
            }

            // 3. Fallback (opcional): lanzar error si no se pudo determinar
            //throw new UnauthorizedAccessException("No se pudo resolver el TenantId.");
            await _next(context);
        }
    }
}
