namespace ServiPuntos.Core.Interfaces
{
    public interface ITenantProvider
    {
        Tenant CurrentTenant { get; }
    }
}
