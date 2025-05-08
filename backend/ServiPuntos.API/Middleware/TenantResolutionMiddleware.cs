using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ServiPuntos.API.Middleware
{
    public class TenantResolutionMiddleware
    {
        private readonly RequestDelegate _next;
        public TenantResolutionMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            // Es un ejemplo de como extrae el tenant de la URL
            // Desp vemos como implementamos en los JWT
            var hostParts = context.Request.Host.Host.Split('.');
            var tenantName = hostParts[0];
            context.Request.Headers["X-Tenant-Name"] = tenantName;
            await _next(context);
        }
    }
}
