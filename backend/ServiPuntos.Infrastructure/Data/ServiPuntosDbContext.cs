using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Infrastructure.MultiTenancy;

namespace ServiPuntos.Infrastructure.Data
{
    public class ServiPuntosDbContext : DbContext
    {
        private readonly ITenantContext _tenantContext;

        public ServiPuntosDbContext(
            DbContextOptions<ServiPuntosDbContext> options,
            ITenantContext tenantContext)
            : base(options)
        {
            _tenantContext = tenantContext;
        }

        // DbSets
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<ProductoCanjeable> ProductosCanjeables { get; set; }
        public DbSet<ProductoUbicacion> ProductoUbicaciones { get; set; }
        public DbSet<Promocion> Promociones { get; set; }
        public DbSet<Ubicacion> Ubicaciones { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Transaccion> Transacciones { get; set; }
        public DbSet<Canje> Canjes { get; set; }
        public DbSet<SaldoPuntos> SaldosPuntos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Usuario → Tenant (1:N)
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Tenant)
                .WithMany(t => t.Usuarios)
                .HasForeignKey(u => u.TenantId);

            // ProductoUbicacion → ProductoCanjeable (N:1)
            modelBuilder.Entity<ProductoUbicacion>()
                .HasOne(pu => pu.ProductoCanjeable)
                .WithMany(p => p.DisponibilidadesPorUbicacion)
                .HasForeignKey(pu => pu.ProductoCanjeableId);

            // ProductoUbicacion → Ubicacion (N:1)
            modelBuilder.Entity<ProductoUbicacion>()
                .HasOne(pu => pu.Ubicacion)
                .WithMany(u => u.ProductosLocales)   // ahora existe ProductosLocales
                .HasForeignKey(pu => pu.UbicacionId);

            // Ubicacion → Tenant (N:1)
            modelBuilder.Entity<Ubicacion>()
                .HasOne(u => u.Tenant)
                .WithMany(t => t.Ubicaciones)
                .HasForeignKey(u => u.TenantId);

            // Promocion ↔ Ubicacion (M:N)
            modelBuilder.Entity<Promocion>()
                .HasMany(p => p.Ubicaciones)
                .WithMany(u => u.Promociones);       // ahora existe Promociones

            // Transaccion: relaciones y restricciones
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

            // Canje: relaciones
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

            // SaldoPuntos: usuario y tenant
            modelBuilder.Entity<SaldoPuntos>()
                .HasOne(s => s.Usuario)
                .WithMany()
                .HasForeignKey(s => s.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SaldoPuntos>()
                .HasOne(s => s.Tenant)
                .WithMany()
                .HasForeignKey(s => s.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices
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

            modelBuilder.Entity<SaldoPuntos>()
                .HasIndex(s => new { s.UsuarioId, s.TenantId })
                .IsUnique();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Puedes descomentar e inyectar TenantId automáticamente si lo deseas:
            /*
            foreach (var entry in ChangeTracker.Entries()
                .Where(e =>
                    e.State == EntityState.Added &&
                    e.Metadata.FindProperty("TenantId") != null &&
                    e.Property("TenantId").CurrentValue == null))
            {
                entry.Property("TenantId").CurrentValue = _tenantContext.TenantId;
            }
            */

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
