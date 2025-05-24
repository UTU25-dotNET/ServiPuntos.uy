using Microsoft.EntityFrameworkCore;
using ServiPuntos.Core.Entities;

namespace ServiPuntos.Infrastructure.Data
{
    public class TenantConfigurationContext : DbContext
    {
        public TenantConfigurationContext(DbContextOptions<TenantConfigurationContext> opts)
            : base(opts) { }

        public DbSet<Tenant> Tenants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tenant>()
                .HasKey(t => t.Id);
            modelBuilder.Entity<Tenant>()
                .HasIndex(t => t.Nombre)
                .IsUnique();
        }
    }
}
