using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Enums;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Core.NAFTA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServiPuntos.Application.Services
{
    public class CanjeService : ICanjeService
    {
        private readonly ICanjeRepository _canjeRepository;
        private readonly IPuntosService _puntosService;
        private readonly IUsuarioService _usuarioService;
        private readonly IProductoCanjeableService _productoCanjeableService;
        private readonly IProductoUbicacionService _productoUbicacionService;
        private readonly IAudienciaService _audienciaService;

        public CanjeService(
            ICanjeRepository canjeRepository,
            IPuntosService puntosService,
            IUsuarioService usuarioService,
            IProductoCanjeableService productoCanjeableService,
            IProductoUbicacionService productoUbicacionService,
            IAudienciaService audienciaService)
        {
            _canjeRepository = canjeRepository;
            _puntosService = puntosService;
            _usuarioService = usuarioService;
            _productoCanjeableService = productoCanjeableService;
            _productoUbicacionService = productoUbicacionService;
            _audienciaService = audienciaService;
        }

        public async Task<Canje> GetCanjeByIdAsync(Guid id)
        {
            return await _canjeRepository.GetByIdAsync(id);
        }

        public async Task<Canje> GetCanjeByCodigoQRAsync(string codigoQR)
        {
            return await _canjeRepository.GetByCodigoQRAsync(codigoQR);
        }

        public async Task<IEnumerable<Canje>> GetCanjesByUsuarioIdAsync(Guid usuarioId)
        {
            return await _canjeRepository.GetByUsuarioIdAsync(usuarioId);
        }

        public async Task<IEnumerable<Canje>> GetCanjesByTenantIdAsync(Guid tenantId)
        {
            return await _canjeRepository.GetByTenantIdAsync(tenantId);
        }

        public async Task<IEnumerable<Canje>> GetCanjesByUbicacionIdAsync(Guid ubicacionId)
        {
            return await _canjeRepository.GetByUbicacionIdAsync(ubicacionId);
        }

        public async Task<IEnumerable<Canje>> GetCanjesPendientesByUsuarioIdAsync(Guid usuarioId)
        {
            return await _canjeRepository.GetPendientesByUsuarioIdAsync(usuarioId);
        }

        public async Task<IEnumerable<Canje>> GetCanjesPendientesByUbicacionIdAsync(Guid ubicacionId)
        {
            return await _canjeRepository.GetPendientesByUbicacionIdAsync(ubicacionId);
        }

        public async Task<bool> ConfirmarCanjeAsync(Guid canjeId)
        {
            var canje = await _canjeRepository.GetByIdAsync(canjeId);
            if (canje == null)
                throw new Exception("Canje no encontrado");

            if (canje.Estado != EstadoCanje.Generado)
                throw new Exception("El canje no se puede confirmar");

            if (canje.FechaExpiracion < DateTime.UtcNow)
                throw new Exception("El canje ha expirado");

            canje.Estado = EstadoCanje.Canjeado;
            canje.FechaCanje = DateTime.UtcNow;

            // Descontar stock si existe registro
            var productos = await _productoUbicacionService.GetAllAsync(canje.UbicacionId);
            var prodUbic = productos.FirstOrDefault(p => p.ProductoCanjeableId == canje.ProductoCanjeableId);
            if (prodUbic != null && prodUbic.StockDisponible > 0)
            {
                prodUbic.StockDisponible -= 1;
                await _productoUbicacionService.UpdateAsync(prodUbic);
            }

            var actualizado = await _canjeRepository.UpdateAsync(canje);

            if (actualizado)
            {
                var usuario = await _usuarioService.GetUsuarioAsync(canje.UsuarioId);
                if (usuario != null)
                {
                    await _audienciaService.ActualizarSegmentosUsuariosAsync(canje.TenantId, new List<Usuario> { usuario });
                }
            }

            return actualizado;
        }

        public async Task<string> GenerarCodigoCanjeAsync(Guid usuarioId, Guid productoCanjeableId, Guid ubicacionId, Guid tenantId)
        {
            // Verificar que el producto existe
            var producto = await _productoCanjeableService.GetProductoAsync(productoCanjeableId);
            if (producto == null)
            {
                throw new Exception($"Producto canjeable con ID {productoCanjeableId} no encontrado");
            }

            // Verificar que el usuario tiene suficientes puntos
            int saldoUsuario = await _puntosService.GetSaldoByUsuarioIdAsync(usuarioId);
            if (saldoUsuario < producto.CostoEnPuntos)
            {
                throw new Exception("Saldo insuficiente para realizar el canje");
            }

            // Generar código QR único
            string codigoQR = GenerarCodigoQRUnico();

            // Crear registro de canje
            var canje = new Canje
            {
                UsuarioId = usuarioId,
                UbicacionId = ubicacionId,
                TenantId = tenantId,
                ProductoCanjeableId = productoCanjeableId,
                CodigoQR = codigoQR,
                FechaGeneracion = DateTime.UtcNow,
                FechaExpiracion = DateTime.UtcNow.AddHours(24), // Expiración en 24 horas
                Estado = EstadoCanje.Generado,
                PuntosCanjeados = producto.CostoEnPuntos
            };

            // Reservar los puntos (se debitarán al completar el canje)
            await _puntosService.DebitarPuntosAsync(usuarioId, producto.CostoEnPuntos);


            var usuario = await _usuarioService.GetUsuarioAsync(usuarioId);
            if (usuario != null)
            {
                await _audienciaService.ActualizarSegmentosUsuariosAsync(tenantId, new List<Usuario> { usuario });
            }
            // Guardar el canje
            await _canjeRepository.AddAsync(canje);

            return codigoQR;
        }

        public async Task<bool> ProcesarCanjeAsync(CanjeNAFTA canjeNAFTA)
        {
            // Buscar el canje por código QR
            var canje = await _canjeRepository.GetByCodigoQRAsync(canjeNAFTA.CodigoQR);
            if (canje == null)
            {
                throw new Exception($"Canje con código QR {canjeNAFTA.CodigoQR} no encontrado");
            }

            // Verificar que el canje no ha expirado
            if (canje.FechaExpiracion < DateTime.UtcNow)
            {
                throw new Exception("El código de canje ha expirado");
            }

            // Verificar que el canje no ha sido utilizado
            if (canje.Estado != EstadoCanje.Generado)
            {
                throw new Exception("El código de canje ya ha sido utilizado o cancelado");
            }

            // Verificar que la ubicación coincide (si se especifica)
            if (canjeNAFTA.UbicacionId != null && (canjeNAFTA.UbicacionId) != canje.UbicacionId)
            {
                throw new Exception("El canje debe realizarse en la ubicación especificada");
            }

            // Actualizar el estado del canje
            canje.Estado = EstadoCanje.Canjeado;
            canje.FechaCanje = DateTime.UtcNow;
            // Descontar stock del producto en la ubicación
            var productos = await _productoUbicacionService.GetAllAsync(canje.UbicacionId);
            var prodUbic = productos.FirstOrDefault(p => p.ProductoCanjeableId == canje.ProductoCanjeableId);
            if (prodUbic != null && prodUbic.StockDisponible > 0)
            {
                prodUbic.StockDisponible -= 1;
                await _productoUbicacionService.UpdateAsync(prodUbic);
            }
            // Guardar cambios
            var actualizado = await _canjeRepository.UpdateAsync(canje);

            if (actualizado)
            {
                var usuario = await _usuarioService.GetUsuarioAsync(canje.UsuarioId);
                if (usuario != null)
                {
                    await _audienciaService.ActualizarSegmentosUsuariosAsync(canje.TenantId, new List<Usuario> { usuario });
                }
            }

            return actualizado;
        }

        public async Task<IEnumerable<Canje>> GetCanjesByUsuarioIdPaginatedAsync(Guid usuarioId, Guid? cursor, int limit)
        {
            return await _canjeRepository.GetByUsuarioIdPaginatedAsync(usuarioId, cursor, limit);
        }

        public async Task<bool> ValidarCanjeAsync(string codigoQR)
        {
            var canje = await _canjeRepository.GetByCodigoQRAsync(codigoQR);
            if (canje == null)
            {
                return false;
            }

            return canje.Estado == EstadoCanje.Generado && canje.FechaExpiracion > DateTime.UtcNow;
        }

        private string GenerarCodigoQRUnico()
        {
            // Generar un código QR único basado en GUID y timestamp
            string input = Guid.NewGuid().ToString() + DateTime.Now.Ticks.ToString();

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(hashBytes)
                    .Replace("/", "_")
                    .Replace("+", "-")
                    .Substring(0, 20); // Limitar a 20 caracteres para que sea más manejable
            }
        }
    }
}