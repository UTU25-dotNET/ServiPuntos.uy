using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiPuntos.Core.Entities;

namespace ServiPuntos.Core.Interfaces
{
    public interface IUbicacionService
    {
        //GET
        Task<Ubicacion?> GetUbicacionAsync(Guid idUbicacion);
        Task<Ubicacion?> GetUbicacionAsync(string nombre);
        Task<Ubicacion?> GetUbicacionAsync(Guid tenantId, Guid idUbicacion);
        //GET ALL
        Task<IEnumerable<Ubicacion>> GetAllUbicacionesAsync();
        Task<IEnumerable<Ubicacion>> GetAllUbicacionesAsync(Guid tenantId);
        //ADD
        Task AddUbicacionAsync(Ubicacion ubicacion);
        Task AddUbicacionAsync(Guid tenantId, Ubicacion ubicacion);
        //UPDATE
        Task UpdateUbicacionAsync(Ubicacion ubicacion);
        Task UpdateUbicacionByTenantAsync(Guid tenantId, Ubicacion ubicacion);
        //DELETE
        Task DeleteUbicacionAsync(Guid idUbicacion);
        Task DeleteUbicacionAsync(Guid tenantId, Guid idUbicacion);
    }
}
