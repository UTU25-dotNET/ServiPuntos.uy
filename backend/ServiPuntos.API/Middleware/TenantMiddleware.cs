using ServiPuntos.Core.Interfaces;

namespace ServiPuntos.API.Middleware
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
    }
}
