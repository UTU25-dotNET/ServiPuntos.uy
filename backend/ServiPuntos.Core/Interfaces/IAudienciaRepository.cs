// Namespace: ServiPuntos.Core.Interfaces
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiPuntos.Core.Entities;

namespace ServiPuntos.Core.Interfaces
{
    public interface IAudienciaRepository
    {
        /// <summary>
        /// Obtiene una audiencia por su ID, incluyendo sus reglas.
        /// </summary>
        Task<Audiencia?> GetByIdWithReglasAsync(Guid audienciaId);

        /// <summary>
        /// Obtiene una audiencia por su NombreUnicoInterno para un tenant específico, incluyendo sus reglas.
        /// </summary>
        Task<Audiencia?> GetByNombreUnicoWithReglasAsync(Guid tenantId, string nombreUnicoInterno);

        /// <summary>
        /// Lista todas las audiencias para un tenant, incluyendo sus reglas.
        /// Opcionalmente filtra por estado Activa y ordena por Prioridad.
        /// </summary>
        Task<IEnumerable<Audiencia>> ListByTenantIdWithReglasAsync(Guid tenantId, bool soloActivas = false, bool ordenarPorPrioridad = false);

        /// <summary>
        /// Añade una nueva audiencia (y sus reglas) a la base de datos.
        /// </summary>
        Task AddAsync(Audiencia audiencia);

        /// <summary>
        /// Actualiza una audiencia existente.
        /// Esto debe incluir la lógica para sincronizar la colección de Reglas
        /// (añadir nuevas, actualizar existentes, eliminar las que ya no están).
        /// </summary>
        Task UpdateAsync(Audiencia audiencia);

        /// <summary>
        /// Elimina una audiencia y sus reglas asociadas.
        /// </summary>
        Task DeleteAsync(Guid audienciaId);

        /// <summary>
        /// Verifica si existe una audiencia con el mismo NombreUnicoInterno para un tenant,
        /// excluyendo opcionalmente un ID de audiencia específico (útil al actualizar).
        /// </summary>
        Task<bool> ExisteNombreUnicoAsync(Guid tenantId, string nombreUnicoInterno, Guid? excluirAudienciaId = null);
    }
}