using ServiPuntos.Core.Entities; // Add this if Tenant is defined in this namespace

namespace ServiPuntos.Core.Interfaces

{
    public interface ITenantProvider
    {
        Tenant CurrentTenant { get; }
    }
}
