using Microsoft.AspNetCore.Http;

namespace ServiPuntos.Infrastructure.MultiTenancy
{
    public class TenantResolver : ITenantResolver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetCurrentTenantId()
        {
            var tenantClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("tenantId");

            if (tenantClaim == null || !Guid.TryParse(tenantClaim.Value, out var tenantId))
                throw new UnauthorizedAccessException("TenantId inválido o ausente.");

            return tenantId;
        }
    }
}
