using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Enums;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Core.NAFTA;
using System;
using System.Collections.Generic;
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

        public TransaccionService(
            ITransaccionRepository transaccionRepository,
            IPuntosService puntosService,
            IPointsRuleEngine pointsRuleEngine,
            IUsuarioService usuarioService)
        {
            _transaccionRepository = transaccionRepository;
            _puntosService = puntosService;
            _pointsRuleEngine = pointsRuleEngine;
            _usuarioService = usuarioService;
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
                Detalles = JsonSerializer.Serialize(transaccionNAFTA.Productos)
            };

            // Registrar la transacción
            await _transaccionRepository.AddAsync(transaccion);

            // Actualizar el saldo de puntos del usuario
            await _puntosService.ActualizarSaldoAsync(usuario.Id, puntosOtorgados);

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
            return await _transaccionRepository.AddAsync(transaccion);
        }
    }
}