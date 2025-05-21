using System;
using System.Threading.Tasks;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Core.Entities;

namespace ServiPuntos.Application.Services
{
    public class PuntosService : IPuntosService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public PuntosService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<int> GetSaldoByUsuarioIdAsync(Guid usuarioId)
        {
            var usuario = await _usuarioRepository.GetAsync(usuarioId);
            if (usuario == null)
            {
                throw new Exception($"Usuario con ID {usuarioId} no encontrado");
            }

            return usuario.Puntos;
        }

        public async Task ActualizarSaldoAsync(Guid usuarioId, int puntosAgregar)
        {
            var usuario = await _usuarioRepository.GetAsync(usuarioId);
            if (usuario == null)
            {
                throw new Exception($"Usuario con ID {usuarioId} no encontrado");
            }

            usuario.Puntos += puntosAgregar;
            await _usuarioRepository.UpdateAsync(usuario);
        }

        public async Task DebitarPuntosAsync(Guid usuarioId, int puntosDebitar)
        {
            if (puntosDebitar <= 0)
            {
                throw new ArgumentException("La cantidad de puntos a debitar debe ser mayor que cero");
            }

            var usuario = await _usuarioRepository.GetAsync(usuarioId);
            if (usuario == null)
            {
                throw new Exception($"Usuario con ID {usuarioId} no encontrado");
            }

            if (usuario.Puntos < puntosDebitar)
            {
                throw new InvalidOperationException("Saldo insuficiente para realizar el débito");
            }

            usuario.Puntos -= puntosDebitar;
            await _usuarioRepository.UpdateAsync(usuario);
        }
    }
}