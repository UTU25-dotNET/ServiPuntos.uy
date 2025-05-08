namespace ServiPuntos.Infrastructure.MultiTenancy
{
    public class TenantContext : ITenantContext
    {
        public Guid TenantId { get; set; }
    }
}
