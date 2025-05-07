
using Microsoft.EntityFrameworkCore;

public class ServiPuntosDbContext : DbContext
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public ServiPuntosDbContext(DbContextOptions<ServiPuntosDbContext> options)
    : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<Usuario>().ToTable("Usuarios");
    }
}
