using ServiPuntos.Core.Entities; // Add this line if Tenant is defined in Models namespace

namespace ServiPuntos.Core.Interfaces
{
    public interface ITenantProvider
    {
        Tenant CurrentTenant { get; }
    }
}
