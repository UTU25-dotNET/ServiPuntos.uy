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
        public DbSet<Audiencia> Audiencias { get; set; }
        public DbSet<ReglaAudiencia> ReglasAudiencia { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<ProductoCanjeable> ProductosCanjeables { get; set; }
        public DbSet<ProductoUbicacion> ProductoUbicaciones { get; set; }
        public DbSet<Promocion> Promociones { get; set; }
        public DbSet<Ubicacion> Ubicaciones { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Transaccion> Transacciones { get; set; }
        public DbSet<Canje> Canjes { get; set; }

        public DbSet<ConfigPlataforma> ConfigPlataformas { get; set; }

        // SEEDER 
        //public DbSet<OperadorDisponible> OperadoresDisponibles { get; set; }
        //public DbSet<CampoDisponible> CamposDisponibles { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Intereses)
                .IsRequired(false);

            // Relación Usuario – Tenant (1:N)
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Tenant)
                .WithMany(t => t.Usuarios)
                .HasForeignKey(u => u.TenantId);

            // Relación Usuario – Ubicacion (N:1 opcional)
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Ubicacion)
                .WithMany()
                .HasForeignKey(u => u.UbicacionId)
                .OnDelete(DeleteBehavior.Restrict);

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

            modelBuilder.Entity<Transaccion>()
                .HasOne(t => t.Usuario)
                .WithMany()
                .HasForeignKey(t => t.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaccion>()
                .HasOne(t => t.Ubicacion)
                .WithMany()
                .HasForeignKey(t => t.UbicacionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaccion>()
                .HasOne(t => t.Tenant)
                .WithMany()
                .HasForeignKey(t => t.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Canje>()
                .HasOne(c => c.Usuario)
                .WithMany()
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Canje>()
                .HasOne(c => c.Ubicacion)
                .WithMany()
                .HasForeignKey(c => c.UbicacionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Canje>()
                .HasOne(c => c.Tenant)
                .WithMany()
                .HasForeignKey(c => c.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Canje>()
                .HasOne(c => c.ProductoCanjeable)
                .WithMany()
                .HasForeignKey(c => c.ProductoCanjeableId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices para mejor rendimiento
            modelBuilder.Entity<Transaccion>()
                .HasIndex(t => t.UsuarioId);

            modelBuilder.Entity<Transaccion>()
                .HasIndex(t => t.UbicacionId);

            modelBuilder.Entity<Transaccion>()
                .HasIndex(t => t.FechaTransaccion);

            modelBuilder.Entity<Canje>()
                .HasIndex(c => c.CodigoQR)
                .IsUnique();

            modelBuilder.Entity<Canje>()
                .HasIndex(c => c.UsuarioId);
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