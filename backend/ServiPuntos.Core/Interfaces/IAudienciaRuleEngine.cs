// Namespace: ServiPuntos.Core.Interfaces
// Propósito: Define el contrato para el motor de reglas de audiencias.
//            Su responsabilidad es determinar a qué audiencia pertenece un usuario.

using System.Collections.Generic;
using System.Threading.Tasks; // Async por si ObtenerDatosTransaccionesUsuarioAsync es parte de él
using ServiPuntos.Core.DTOs;
using ServiPuntos.Core.Entities;

namespace ServiPuntos.Core.Interfaces
{
    public interface IAudienciaRuleEngine
    {
        /// <summary>
        /// Clasifica un usuario contra una lista de definiciones de audiencia (ya ordenadas por prioridad).
        /// </summary>
        /// <param name="usuario">El usuario a clasificar.</param>
        /// <param name="audienciasDefinidas">Lista de audiencias activas y ordenadas por prioridad del tenant.</param>
        /// <param name="datosTransacciones">Datos de transacciones pre-cargados para el usuario (opcional pero recomendado para optimizar).</param>
        /// <returns>El NombreUnicoInterno de la Audiencia a la que pertenece el usuario, o un ID de segmento por defecto.</returns>
        Task<Guid?> ClasificarUsuarioAsync(Usuario usuario,IEnumerable<Audiencia> audienciasDefinidas,DatosTransaccionesUsuario datosTransacciones );// Se lo pasamos para que no tenga que obtenerlo él
    }

    // Podrías mover DatosTransaccionesUsuario aquí si es específico del RuleEngine
    // o mantenerlo en Entities o DTOs si es más general.
    // Por ahora, asumamos que está definido en otro lugar (como en tu código original).
    // public class DatosTransaccionesUsuario { /* ... */ }
}