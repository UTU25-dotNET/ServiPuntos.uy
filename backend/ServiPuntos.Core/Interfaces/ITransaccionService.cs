using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.NAFTA;

namespace ServiPuntos.Core.Interfaces
{
    public interface ITransaccionService
    {
        Task<Transaccion> GetTransaccionByIdAsync(int id);
        Task<IEnumerable<Transaccion>> GetTransaccionesByUsuarioIdAsync(int usuarioId);
        Task<IEnumerable<Transaccion>> GetTransaccionesByUbicacionIdAsync(int ubicacionId);
        Task<IEnumerable<Transaccion>> GetTransaccionesByTenantIdAsync(int tenantId);
        Task<IEnumerable<Transaccion>> GetTransaccionesByDateRangeAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<RespuestaPuntosNAFTA> ProcesarTransaccionNAFTAAsync(TransaccionNAFTA transaccion, int tenantId, int ubicacionId);
        Task<int> RegistrarTransaccionAsync(Transaccion transaccion);
    }
}