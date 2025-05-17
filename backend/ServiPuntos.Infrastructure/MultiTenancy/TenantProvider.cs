using ServiPuntos.Core.Interfaces;
using ServiPuntos.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using ServiPuntos.Core.Entities;


namespace ServiPuntos.Infrastructure.MultiTenancy
{
    public class TenantProvider : ITenantProvider
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly TenantConfigurationContext _configContext;

        public TenantProvider(IHttpContextAccessor http, TenantConfigurationContext config)
        {
            _httpContext = http;
            _configContext = config;
        }

        public Tenant CurrentTenant
        {
            get
            {
                var tenantName = _httpContext.HttpContext.Request.Headers["X-Tenant-Name"].FirstOrDefault();
                return _configContext.Tenants.SingleOrDefault(t => t.Nombre == tenantName);
            }
        }
    }
}