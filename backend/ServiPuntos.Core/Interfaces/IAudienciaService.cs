using ServiPuntos.Core.DTOs;
using ServiPuntos.Core.Entities;
namespace ServiPuntos.Core.Interfaces
{
    public interface IAudienciaService
    {
        Task<Audiencia> GuardarDefinicionAudienciaAsync(Guid tenantId, AudienciaDto dto);
        Task EliminarDefinicionAudienciaAsync(Guid tenantId, Guid audienciaId);
        Task ActualizarSegmentosUsuariosAsync(Guid tenantId, List<Usuario> usuariosParaActualizar);
        Task<string> ClasificarUsuarioAsync(Guid usuarioId, Guid tenantId); // Devuelve NombreUnicoInterno o null
        Task<IEnumerable<Usuario>> GetUsuariosPorAudienciaAsync(Guid tenantId, string nombreUnicoAudiencia);
        Task<Dictionary<string, IEnumerable<Usuario>>> GetTodasLasAudienciasConUsuariosAsync(Guid tenantId);
        Task<Dictionary<string, int>> GetDistribucionUsuariosPorAudienciaAsync(Guid tenantId);
        Task<EstadisticasAudienciaDinamica> GetEstadisticasGlobalesYAporAudienciaAsync(Guid tenantId);
        Task<IEnumerable<Audiencia>> GetDefinicionesAudienciaAsync(Guid tenantId);
    }
}