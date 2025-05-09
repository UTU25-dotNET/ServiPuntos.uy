/*using ServiPuntos.Core.Interfaces;
using ServiPuntos.Infrastructure.Data;

namespace ServiPuntos.Infrastructure.MultiTenancy
{
    public class TenantProvider : ITenantProvider
    {
        private readonly IHttpContextAccessor _http;
        private readonly TenantConfigurationContext _config;

        public TenantProvider(IHttpContextAccessor http, TenantConfigurationContext config)
        {
            _http = http;
            _config = config;
        }

        public Tenant CurrentTenant
        {
            get
            {
                var tenantName = _http.HttpContext.Request.Headers["X-Tenant-Name"].FirstOrDefault();
                return _config.Tenants.SingleOrDefault(t => t.Name == tenantName);
            }
        }
    }
}*/