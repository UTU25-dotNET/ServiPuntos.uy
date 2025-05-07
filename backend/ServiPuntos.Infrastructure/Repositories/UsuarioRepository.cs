using Microsoft.EntityFrameworkCore;
public class UsuarioRepository : IUsuarioRepository
{
    private readonly ServiPuntosDbContext _dbContext;

    public UsuarioRepository(ServiPuntosDbContext context)
    {
        _dbContext = context;
    }

    public async Task AddAsync(Usuario usuario)
    {
        await _dbContext.Usuarios.AddAsync(usuario);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Usuario?> GetByIdAsync(Guid id)
        => await _dbContext.Usuarios.FindAsync(id);

    public async Task<IEnumerable<Usuario>> GetAllAsync()
        => await _dbContext.Usuarios.ToListAsync();

    public async Task UpdateAsync(Usuario usuario)
    {
        _dbContext.Usuarios.Update(usuario);
        await _dbContext.SaveChangesAsync();
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
