using Microsoft.EntityFrameworkCore;
using ServiPuntos.Core.Entities;
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
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<ProductoCanjeable> ProductosCanjeables { get; set; }
        public DbSet<ProductoUbicacion> ProductoUbicaciones { get; set; }
        public DbSet<Promocion> Promociones { get; set; }
        public DbSet<Ubicacion> Ubicaciones { get; set; }
        public DbSet<Tenant> Tenants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relación Usuario – Tenant (1:N)
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Tenant)
                .WithMany(t => t.Usuarios)
                .HasForeignKey(u => u.TenantId);

            // Relación ProductoUbicacion – ProductoCanjeable (N:1)
            modelBuilder.Entity<ProductoUbicacion>()
                .HasOne(pu => pu.ProductoCanjeable)
                .WithMany(p => p.DisponibilidadesPorUbicacion)
                .HasForeignKey(pu => pu.ProductoCanjeableId);

            // Relación ProductoUbicacion – Ubicacion (N:1)
            modelBuilder.Entity<ProductoUbicacion>()
                .HasOne(pu => pu.Ubicacion)
                .WithMany(u => u.ProductosLocales)
                .HasForeignKey(pu => pu.UbicacionId);

            // Relación Ubicacion – Tenant (N:1)
            modelBuilder.Entity<Ubicacion>()
                .HasOne(u => u.Tenant)
                .WithMany(t => t.Ubicaciones)
                .HasForeignKey(u => u.TenantId);

            // Relación muchos a muchos Promocion – Ubicacion
            modelBuilder.Entity<Ubicacion>()
                .HasMany(u => u.Promociones)
                .WithMany(p => p.Ubicaciones);

            // Filtro global por TenantId para las entidades que lo tienen
            //modelBuilder.Entity<Usuario>()
            //.HasQueryFilter(u => u.TenantId == _iTenantContext.TenantId);

            //modelBuilder.Entity<Ubicacion>() // si corresponde
            //.HasQueryFilter(u => u.TenantId == _tenantProvider.CurrentTenant.Id);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries()
              .Where(e =>
                  e.State == EntityState.Added &&
                  e.Metadata.FindProperty("TenantId") != null &&
                  e.Property("TenantId").CurrentValue == null))
            {
                // Inyecta tenantid automáticamente a todas las entidades nuevas que lo necesiten, antes de persistirlas
                Console.WriteLine(_iTenantContext);
                //entry.Property("TenantId").CurrentValue = _iTenantContext.TenantId;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}