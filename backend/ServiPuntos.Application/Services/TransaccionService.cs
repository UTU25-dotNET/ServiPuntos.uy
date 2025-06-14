using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Enums;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Core.NAFTA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServiPuntos.Application.Services
{
    public class TransaccionService : ITransaccionService
    {
        private readonly ITransaccionRepository _transaccionRepository;
        private readonly IPuntosService _puntosService;
        private readonly IPointsRuleEngine _pointsRuleEngine;
        private readonly IUsuarioService _usuarioService;
        private readonly IProductoUbicacionService _productoUbicacionService;

        public TransaccionService(
            ITransaccionRepository transaccionRepository,
            IPuntosService puntosService,
            IPointsRuleEngine pointsRuleEngine,
            IUsuarioService usuarioService,
            IProductoUbicacionService productoUbicacionService)
        {
            _transaccionRepository = transaccionRepository;
            _puntosService = puntosService;
            _pointsRuleEngine = pointsRuleEngine;
            _usuarioService = usuarioService;
            _productoUbicacionService = productoUbicacionService;
        }

        public async Task<Transaccion> GetTransaccionByIdAsync(Guid id)
        {
            return await _transaccionRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Transaccion>> GetTransaccionesByUsuarioIdAsync(Guid usuarioId)
        {
            return await _transaccionRepository.GetByUsuarioIdAsync(usuarioId);
        }

        public async Task<IEnumerable<Transaccion>> GetTransaccionesByUbicacionIdAsync(Guid ubicacionId)
        {
            return await _transaccionRepository.GetByUbicacionIdAsync(ubicacionId);
        }

        public async Task<IEnumerable<Transaccion>> GetTransaccionesByTenantIdAsync(Guid tenantId)
        {
            return await _transaccionRepository.GetByTenantIdAsync(tenantId);
        }

        public async Task<IEnumerable<Transaccion>> GetTransaccionesByDateRangeAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _transaccionRepository.GetByDateRangeAsync(fechaInicio, fechaFin);
        }

        public async Task<RespuestaPuntosNAFTA> ProcesarTransaccionNAFTAAsync(TransaccionNAFTA transaccionNAFTA, Guid tenantId, Guid ubicacionId)
        {
            // Buscar el usuario por su identificador
            var usuario = await _usuarioService.GetUsuarioAsync(transaccionNAFTA.IdentificadorUsuario);
            if (usuario == null)
            {
                throw new Exception($"Usuario con identificador {transaccionNAFTA.IdentificadorUsuario} no encontrado");
            }

            // Calcular puntos según las reglas del tenant
            int puntosOtorgados = await _pointsRuleEngine.CalcularPuntosAsync(transaccionNAFTA, tenantId);

            // Obtener saldo anterior
            int saldoAnterior = await _puntosService.GetSaldoByUsuarioIdAsync(usuario.Id);

            // Crear la transacción
            TipoTransaccion tipoTransaccion;
            switch (transaccionNAFTA.TipoTransaccion)
            {
                case TipoTransaccion.CompraCombustible:
                    tipoTransaccion = TipoTransaccion.CompraCombustible;
                    break;
                case TipoTransaccion.CompraMinimercado:
                    tipoTransaccion = TipoTransaccion.CompraMinimercado;
                    break;
                case TipoTransaccion.UsoServicio:
                    tipoTransaccion = TipoTransaccion.UsoServicio;
                    break;
                default:
                    tipoTransaccion = TipoTransaccion.CompraMinimercado;
                    break;
            }

            var transaccion = new Transaccion
            {
                UsuarioId = usuario.Id,
                UbicacionId = ubicacionId,
                TenantId = tenantId,
                FechaTransaccion = transaccionNAFTA.FechaTransaccion,
                TipoTransaccion = tipoTransaccion,
                Monto = transaccionNAFTA.Monto,
                PuntosOtorgados = puntosOtorgados,
                 Detalles = JsonSerializer.Serialize(new
                {
                    Productos = transaccionNAFTA.Productos,
                    DatosAdicionales = transaccionNAFTA.DatosAdicionales
                })
            };

            // Registrar la transacción
            await _transaccionRepository.AddAsync(transaccion);

            // Actualizar el saldo de puntos del usuario
            await _puntosService.ActualizarSaldoAsync(usuario.Id, puntosOtorgados);

            // Descontar stock de los productos involucrados
            await ActualizarStockAsync(ubicacionId, transaccion.Detalles);

            // Obtener saldo actualizado
            int saldoActual = await _puntosService.GetSaldoByUsuarioIdAsync(usuario.Id);

            // Crear respuesta con información de puntos
            return new RespuestaPuntosNAFTA
            {
                IdentificadorUsuario = transaccionNAFTA.IdentificadorUsuario,
                PuntosOtorgados = puntosOtorgados,
                SaldoActual = saldoActual,
                SaldoAnterior = saldoAnterior
            };
        }

        public async Task<Guid> RegistrarTransaccionAsync(Transaccion transaccion)
        {
            // Calcular los puntos a otorgar en base al monto y reglas del tenant
            var productos = new List<LineaTransaccionNAFTA>();
            if (!string.IsNullOrWhiteSpace(transaccion.Detalles))
            {
                try
                {
                    var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var detalles = JsonSerializer.Deserialize<DetallesTransaccion>(transaccion.Detalles, opts);
                    if (detalles?.Productos != null)
                    {
                        productos = detalles.Productos;
                    }
                }
                catch { /* Si el JSON no es válido, ignorar */ }
            }

            var transaccionNafta = new TransaccionNAFTA
            {
                Monto = (int)transaccion.Monto,
                TipoTransaccion = transaccion.TipoTransaccion,
                Productos = productos
            };

            int puntos = await _pointsRuleEngine.CalcularPuntosAsync(transaccionNafta, transaccion.TenantId);
            transaccion.PuntosOtorgados = puntos;

            var id = await _transaccionRepository.AddAsync(transaccion);

            if (puntos > 0)
            {
                await _puntosService.ActualizarSaldoAsync(transaccion.UsuarioId, puntos);
            }

             // Ajustar stock de la ubicación por los productos comprados
            await ActualizarStockAsync(transaccion.UbicacionId, transaccion.Detalles);

            return id;
        
            }

        public async Task<IEnumerable<Transaccion>> GetTransaccionesByUsuarioIdPaginatedAsync(Guid usuarioId, Guid? cursor, int limit)
        {
            return await _transaccionRepository.GetByUsuarioIdPaginatedAsync(usuarioId, cursor, limit);
        }
        
        private async Task ActualizarStockAsync(Guid ubicacionId, string detallesJson)
        {
            if (string.IsNullOrWhiteSpace(detallesJson))
            {
                Console.WriteLine("No hay detalles para actualizar el stock.");
                return;
            }
            try
            {
                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                List<LineaTransaccionNAFTA>? productos = null;

                if (detallesJson.TrimStart().StartsWith("["))
                {
                    // Formato antiguo: solo array de productos
                    productos = JsonSerializer.Deserialize<List<LineaTransaccionNAFTA>>(detallesJson, opts);
                }
                else
                {
                    var detalles = JsonSerializer.Deserialize<DetallesTransaccion>(detallesJson, opts);
                    productos = detalles?.Productos;
                }

                if (productos == null || productos.Count == 0)
                    return;

                var productosUbicacion = await _productoUbicacionService.GetAllAsync(ubicacionId);
                foreach (var linea in productos)
                {
                    var prod = productosUbicacion.FirstOrDefault(p => p.ProductoCanjeableId == linea.IdProducto);
                    if (prod != null)
                    {
                        prod.StockDisponible = Math.Max(0, prod.StockDisponible - linea.Cantidad);
                        await _productoUbicacionService.UpdateAsync(prod);
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error al actualizar el stock.");
                // Ignorar errores de actualización de stock
            }
        }
    }
}
internal class DetallesTransaccion
    {
        public List<LineaTransaccionNAFTA> Productos { get; set; } = new();
    }
