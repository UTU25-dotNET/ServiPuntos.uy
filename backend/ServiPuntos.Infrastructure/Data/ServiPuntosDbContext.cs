using System.Linq;
using Microsoft.EntityFrameworkCore;
using ServiPuntos.Core.Entities;
using ServiPuntos.Infrastructure.Services;

namespace ServiPuntos.Infrastructure.Data
{
    public class ServiPuntosDbContext : DbContext
    {
        private readonly ITenantProvider _tenantProvider;

        public ServiPuntosDbContext(
            DbContextOptions<ServiPuntosDbContext> options,
            ITenantProvider tenantProvider)
            : base(options)
        {
            _tenantProvider = tenantProvider;
        }

        public DbSet<Usuario> Usuarios => Set<Usuario>();
        // …añade aquí todos tus DbSet que llevan TenantId

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplica filtro global para que cada consulta incluya el TenantId
            modelBuilder.Entity<Usuario>()
                .HasQueryFilter(u => u.TenantId == _tenantProvider.CurrentTenant.Id);

            // …y así para cada entidad que tenga TenantId
        }

        public override int SaveChanges()
        {
            // Al insertar, asigna automáticamente el TenantId
            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added
                         && e.Property("TenantId") != null))
            {
                entry.Property("TenantId").CurrentValue =
                    _tenantProvider.CurrentTenant.Id;
            }
            return base.SaveChanges();
        }
    }
}
