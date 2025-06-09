using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.NAFTA;

namespace ServiPuntos.Core.Interfaces
{
    public interface ICanjeService
    {
        Task<Canje> GetCanjeByIdAsync(Guid id);
        Task<Canje> GetCanjeByCodigoQRAsync(string codigoQR);
        Task<IEnumerable<Canje>> GetCanjesByUsuarioIdAsync(Guid usuarioId);
        Task<IEnumerable<Canje>> GetCanjesByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<Canje>> GetCanjesByUbicacionIdAsync(Guid ubicacionId);
        Task<IEnumerable<Canje>> GetCanjesPendientesByUsuarioIdAsync(Guid usuarioId);
        Task<IEnumerable<Canje>> GetCanjesPendientesByUbicacionIdAsync(Guid ubicacionId);
        Task<bool> ConfirmarCanjeAsync(Guid canjeId);
        Task<string> GenerarCodigoCanjeAsync(Guid usuarioId, Guid productoCanjeableId, Guid ubicacionId, Guid tenantId);
        Task<bool> ProcesarCanjeAsync(CanjeNAFTA canje);
        Task<bool> ValidarCanjeAsync(string codigoQR);
    }
}