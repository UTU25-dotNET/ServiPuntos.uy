using ServiPuntos.Core.Interfaces;

public class TenantService : ITenantService
{
    private readonly ITenantRepository _iTenantRepo;

    public TenantService(ITenantRepository repo)
    {
        _iTenantRepo = repo;
    }
    public async Task AddAsync(Tenant tenant)
        => await _iTenantRepo.AddAsync(tenant);
    public async Task UpdateAsync(Tenant tenant)
        => await _iTenantRepo.UpdateAsync(tenant);
    public async Task DeleteAsync(Guid id)
        => await _iTenantRepo.DeleteAsync(id);

    public async Task<IEnumerable<Tenant>> GetAllAsync()
        => await _iTenantRepo.GetAllAsync();
    public async Task<Tenant?> GetByIdAsync(Guid id)
        => await _iTenantRepo.GetByIdAsync(id);
}
