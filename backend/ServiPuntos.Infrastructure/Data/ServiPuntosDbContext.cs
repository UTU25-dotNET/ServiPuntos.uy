using Microsoft.EntityFrameworkCore;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Infrastructure.MultiTenancy;

namespace ServiPuntos.Infrastructure.Data
{
    public class ServiPuntosDbContext : DbContext
    {
        private readonly ITenantContext _iTenantContext;

        public ServiPuntosDbContext(
            DbContextOptions<ServiPuntosDbContext> options,
            ITenantContext tenantContext)
            : base(options)
        {
            _iTenantContext = tenantContext;
        }

        // DbSets
        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        
        //public DbSet<Ubicacion> Ubicaciones => Set<Ubicacion>(); // si la tenés

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Filtro global por TenantId para las entidades que lo tienen
            modelBuilder.Entity<Usuario>()
                .HasQueryFilter(u => u.TenantId == _iTenantContext.TenantId);

            //modelBuilder.Entity<Ubicacion>() // si corresponde
                //.HasQueryFilter(u => u.TenantId == _tenantProvider.CurrentTenant.Id);
        }

        public override int SaveChanges()
        {

            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added
                 && e.Property("TenantId").CurrentValue == null)) // Solo asignar si TenantId es null
            {
                entry.Property("TenantId").CurrentValue = _iTenantContext.TenantId;
            }

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added
                         && e.Property("TenantId") != null))
            {
                entry.Property("TenantId").CurrentValue = _iTenantContext.TenantId;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
