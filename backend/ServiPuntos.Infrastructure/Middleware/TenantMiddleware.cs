using Microsoft.AspNetCore.Http;
using ServiPuntos.Core.Interfaces;

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
                // No validar tenant si la petición es al endpoint de inicio de sesión o al callback, 
                //no tiene sentido validar tenant en el middelware cuando el usuario no está autenticado aun
                if (context.Request.Path.StartsWithSegments("/api/auth") || context.Request.Path.StartsWithSegments("/api/verify"))
                {
                    await _next(context);
                    return;
                }

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
