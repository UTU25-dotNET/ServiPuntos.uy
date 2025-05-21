using Microsoft.EntityFrameworkCore;


namespace ServiPuntos.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Define tus DbSets (tablas) aquí
        public DbSet<Usuario> Usuarios { get; set; }
        
        // Agrega más DbSets según necesites
        // public DbSet<Producto> Productos { get; set; }
        // public DbSet<Servicio> Servicios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configuraciones específicas para las entidades
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired();
                
                // Índices
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.TenantId); // Índice para búsquedas por tenant
            });
        }
    }
}