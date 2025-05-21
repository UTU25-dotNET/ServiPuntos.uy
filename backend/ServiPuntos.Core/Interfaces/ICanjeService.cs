using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.NAFTA;

namespace ServiPuntos.Core.Interfaces
{
    public interface ICanjeService
    {
        Task<Canje> GetCanjeByIdAsync(int id);
        Task<Canje> GetCanjeByCodigoQRAsync(string codigoQR);
        Task<IEnumerable<Canje>> GetCanjesByUsuarioIdAsync(int usuarioId);
        Task<IEnumerable<Canje>> GetCanjesPendientesByUsuarioIdAsync(int usuarioId);
        Task<string> GenerarCodigoCanjeAsync(int usuarioId, int productoCanjeableId, int ubicacionId, int tenantId);
        Task<bool> ProcesarCanjeAsync(CanjeNAFTA canje);
        Task<bool> ValidarCanjeAsync(string codigoQR);
    }
}