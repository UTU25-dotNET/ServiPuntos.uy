using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;

namespace ServiPuntos.Application.Services
{
    public class DispositivoService : IDispositivoService
    {
        private readonly IDispositivoRepository _repository;

        public DispositivoService(IDispositivoRepository repository)
        {
            _repository = repository;
        }

        public async Task RegistrarTokenAsync(Guid usuarioId, string token)
        {
            var existentes = await _repository.GetByUsuarioAsync(usuarioId);
            if (!existentes.Any(d => d.Token == token))
            {
                await _repository.AddAsync(new DispositivoFcm
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuarioId,
                    Token = token,
                    FechaRegistro = DateTime.UtcNow
                });
            }
        }

        public async Task<List<string>> GetTokensByUsuariosAsync(IEnumerable<Guid> usuarioIds)
        {
            var dispositivos = await _repository.GetByUsuariosAsync(usuarioIds);
            return dispositivos.Select(d => d.Token).Distinct().ToList();
        }
    }
}
