using ServiPuntos.Core.Interfaces;

public class TenantService
{
    private readonly ITenantRepository _iTenantRepo;

    public TenantService(ITenantRepository repo)
    {
        _iTenantRepo = repo;
    }

    public async Task<Tenant?> GetTenantById(Guid id)
        => await _iTenantRepo.GetByIdAsync(id);
}
