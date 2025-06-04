// Namespace: ServiPuntos.Application.Services
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ServiPuntos.Core.DTOs;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Enums;
using ServiPuntos.Core.Interfaces;

namespace ServiPuntos.Application.Services
{
    public class AudienciaService : IAudienciaService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        // private readonly ITenantRepository _tenantRepository; // No es estrictamente necesario si el TenantId se pasa o se obtiene del contexto
        private readonly IAudienciaRepository _audienciaRepository;
        private readonly ITransaccionRepository _transaccionRepository; // Opcional
        private readonly IAudienciaRuleEngine _ruleEngine;
        private readonly ITenantContext _tenantContext; // Para obtener el TenantId si no se pasa
        private readonly ILogger<AudienciaService> _logger;

        // Opcional: GUID conocido para una audiencia "Común" explícita si la tienes.
        // public static readonly Guid? AudienciaComunConocidaId = new Guid("GUID-DE-TU-AUDIENCIA-COMUN");
        private const string KeyParaUsuariosNoAsignados = "_sin_asignar_"; // Clave para usuarios con SegmentoDinamicoId = null

        public AudienciaService(
            IUsuarioRepository usuarioRepository,
            // ITenantRepository tenantRepository,
            IAudienciaRepository audienciaRepository,
            IAudienciaRuleEngine ruleEngine,
            ITenantContext tenantContext,
            ILogger<AudienciaService> logger,
            ITransaccionRepository transaccionRepository)
        {
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            // _tenantRepository = tenantRepository;
            _audienciaRepository = audienciaRepository ?? throw new ArgumentNullException(nameof(audienciaRepository));
            _ruleEngine = ruleEngine ?? throw new ArgumentNullException(nameof(ruleEngine));
            _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _transaccionRepository = transaccionRepository;
        }

        private Guid GetCurrentTenantId(Guid? tenantIdParam)
        {
            var id = tenantIdParam ?? _tenantContext?.TenantId ?? Guid.Empty;
            if (id == Guid.Empty)
            {
                _logger.LogError("TenantId no pudo ser determinado (ni por parámetro ni por contexto).");
                throw new InvalidOperationException("TenantId no pudo ser determinado.");
            }
            return id;
        }
        //GET AUDIENCIA
        public async Task<Audiencia?> GetAudienciaAsync(Guid tenantId, Guid audienciaId)
        {
            tenantId = GetCurrentTenantId(tenantId); // Asegurar TenantId
            _logger.LogInformation("Obteniendo definición de audiencia para TenantId: {TenantId}, AudienciaId: {AudienciaId}", tenantId, audienciaId);
            if (audienciaId == Guid.Empty)
            {
                _logger.LogWarning("GetDefinicionAudienciaAsync: AudienciaId no puede ser Guid.Empty.");
                return null;
            }
            var audiencia = await _audienciaRepository.GetByIdWithReglasAsync(audienciaId);
            if (audiencia == null || audiencia.TenantId != tenantId)
            {
                _logger.LogWarning("GetDefinicionAudienciaAsync: Audiencia con Id {AudienciaId} no encontrada o no pertenece al TenantId {TenantId}.", audienciaId, tenantId);
                return null;
            }
            return audiencia;
        }
        //GET ALL AUDIENCIAS
        public async Task<IEnumerable<Audiencia>> GetAllAudienciasAsync(Guid tenantId)
        {
            tenantId = GetCurrentTenantId(tenantId); // Asegurar TenantId
            _logger.LogInformation("Obteniendo todas las audiencias para TenantId: {TenantId}, SoloActivas: {SoloActivas}", tenantId);
            return await _audienciaRepository.ListByTenantIdWithReglasAsync(tenantId, ordenarPorPrioridad: true);
        }

        
        public async Task<Audiencia> GuardarAudienciaAsync(Guid tenantId, AudienciaDto dto)
        {
            tenantId = GetCurrentTenantId(tenantId); // Asegurar TenantId
            _logger.LogInformation("Guardando definición de audiencia para TenantId: {TenantId}, NombreUnicoInput: {NombreUnicoInput}, AudienciaIdInput: {DtoId}",
                tenantId, dto.NombreUnicoInterno, dto.Id);

            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.NombreUnicoInterno)) throw new ArgumentException("NombreUnicoInterno es requerido.");
            if (string.IsNullOrWhiteSpace(dto.NombreDescriptivo)) throw new ArgumentException("NombreDescriptivo es requerido.");

            Audiencia audienciaDb;
            bool esNuevaAudiencia = dto.Id == Guid.Empty;

            if (await _audienciaRepository.ExisteNombreUnicoAsync(tenantId, dto.NombreUnicoInterno, esNuevaAudiencia ? (Guid?)null : dto.Id))
            {
                _logger.LogError("Guardar Audiencia: NombreUnicoInterno '{NombreUnico}' ya existe para TenantId {TenantId}.", dto.NombreUnicoInterno, tenantId);
                throw new InvalidOperationException($"Audiencia con identificador '{dto.NombreUnicoInterno}' ya existe o conflicto.");
            }

            if (esNuevaAudiencia)
            {
                audienciaDb = new Audiencia { Id = Guid.NewGuid(), TenantId = tenantId, FechaCreacion = DateTime.UtcNow };
                _logger.LogInformation("Creando nueva audiencia con ID: {AudienciaId}", audienciaDb.Id);
            }
            else
            {
                audienciaDb = await _audienciaRepository.GetByIdWithReglasAsync(dto.Id); // Cargar con Reglas para que el repo pueda hacer diff
                if (audienciaDb == null || audienciaDb.TenantId != tenantId)
                {
                    _logger.LogError("Actualizar Audiencia: Audiencia Id {AudId} no encontrada o no pertenece al TenantId {TenantId}.", dto.Id, tenantId);
                    throw new KeyNotFoundException($"Audiencia con Id {dto.Id} no encontrada para el tenant especificado.");
                }
                _logger.LogInformation("Actualizando audiencia existente con ID: {AudienciaId}", audienciaDb.Id);
            }

            // Mapeo de DTO a Entidad
            audienciaDb.NombreUnicoInterno = dto.NombreUnicoInterno;
            audienciaDb.NombreDescriptivo = dto.NombreDescriptivo;
            audienciaDb.Descripcion = dto.Descripcion;
            audienciaDb.Prioridad = dto.Prioridad;
            audienciaDb.Activa = dto.Activa;
            audienciaDb.FechaModificacion = DateTime.UtcNow;

            // Mapear reglas DTO a entidades ReglaAudiencia
            // El repositorio se encargará de la lógica de añadir/actualizar/eliminar reglas hijas.
            var nuevasReglasEntities = new List<ReglaAudiencia>();
            if (dto.Reglas != null)
            {
                foreach (var rDto in dto.Reglas)
                {
                    nuevasReglasEntities.Add(new ReglaAudiencia
                    {
                        Id = rDto.Id, // Si es 0, EF Core lo tratará como nuevo al añadir a la colección de una entidad rastreada
                        AudienciaId = audienciaDb.Id, // Se asigna al guardar la audiencia padre
                        Propiedad = rDto.Propiedad,
                        Operador = rDto.Operador,
                        Valor = rDto.Valor,
                        OperadorLogicoConSiguiente = rDto.OperadorLogicoConSiguiente,
                        OrdenEvaluacion = rDto.OrdenEvaluacion
                    });
                }
            }
            audienciaDb.Reglas = nuevasReglasEntities; // El repositorio debe manejar la sincronización

            if (esNuevaAudiencia)
            {
                await _audienciaRepository.AddAsync(audienciaDb);
            }
            else
            {
                await _audienciaRepository.UpdateAsync(audienciaDb);
            }

            _logger.LogInformation("Definición de audiencia guardada. Iniciando reclasificación para TenantId: {TenantId}.", tenantId);
            await ActualizarSegmentosUsuariosAsync(tenantId, null);
            return await _audienciaRepository.GetByIdWithReglasAsync(audienciaDb.Id); // Devolver la entidad completa desde la BD
        }

        public async Task EliminarAudienciaAsync(Guid tenantId, Guid audienciaId)
        {
            tenantId = GetCurrentTenantId(tenantId);
            _logger.LogInformation("Eliminando AudienciaId: {AudienciaId} para TenantId: {TenantId}", audienciaId, tenantId);

            var audiencia = await _audienciaRepository.GetByIdWithReglasAsync(audienciaId); // Cargar para verificar pertenencia
            if (audiencia == null || audiencia.TenantId != tenantId)
            {
                _logger.LogWarning("Eliminar Audiencia: AudienciaId {AudienciaId} no encontrada o no pertenece al TenantId {TenantId}.", audienciaId, tenantId);
                throw new KeyNotFoundException("Audiencia no encontrada o no pertenece al tenant.");
            }

            await _audienciaRepository.DeleteAsync(audienciaId);
            _logger.LogInformation("Audiencia Id: {AudienciaId} eliminada. Reclasificando usuarios para TenantId: {TenantId}.", audienciaId, tenantId);
            await ActualizarSegmentosUsuariosAsync(tenantId,null);
        }

        public async Task ActualizarSegmentosUsuariosAsync(Guid tenantId, List<Usuario>? usuariosParaActualizar)
        {
            tenantId = GetCurrentTenantId(tenantId);
            _logger.LogInformation("Iniciando ActualizarSegmentosUsuariosAsync para TenantId: {TenantId}", tenantId);

            // Cargar definiciones de audiencia activas y ordenadas del repositorio
            var audienciasDefinidas = (await _audienciaRepository.ListByTenantIdWithReglasAsync(tenantId, soloActivas: true, ordenarPorPrioridad: true))
                                      .ToList();

            List<Usuario> usuariosAProcesar;
            if (usuariosParaActualizar != null)
            {
                usuariosAProcesar = usuariosParaActualizar.Where(u => u.Rol == RolUsuario.UsuarioFinal && u.TenantId == tenantId).ToList();
            }
            else
            {
                usuariosAProcesar = (await _usuarioRepository.GetAllByTenantAsync(tenantId))
                                    .Where(u => u.Rol == RolUsuario.UsuarioFinal).ToList();
            }

            if (!usuariosAProcesar.Any())
            {
                _logger.LogInformation("No hay usuarios finales para procesar en TenantId: {TenantId}.", tenantId);
                return;
            }

            if (!audienciasDefinidas.Any())
            {
                _logger.LogInformation("No hay audiencias activas definidas para TenantId: {TenantId}. Los usuarios procesados tendrán SegmentoDinamicoId = null.", tenantId);
            }

            int contadorActualizados = 0;
            // Considerar paralelizar si es seguro y hay muchos usuarios
            foreach (var usuario in usuariosAProcesar)
            {
                var datosTransacciones = _transaccionRepository != null
                    ? await _ObtenerDatosTransaccionesUsuarioAsync(usuario.Id)
                    : new DatosTransaccionesUsuario();

                Guid? nuevoSegmentoGuid = await _ruleEngine.ClasificarUsuarioAsync(usuario, audienciasDefinidas, datosTransacciones);

                if (usuario.SegmentoDinamicoId != nuevoSegmentoGuid)
                {
                    usuario.SegmentoDinamicoId = nuevoSegmentoGuid;
                    usuario.FechaModificacion = DateTime.UtcNow;
                    await _usuarioRepository.UpdateAsync(usuario);
                    contadorActualizados++;
                }
            }
            _logger.LogInformation("Actualización de segmentos completada para TenantId: {TenantId}. {Contador} usuarios actualizados.", tenantId, contadorActualizados);
        }

        public async Task<string?> ClasificarUsuarioAsync(Guid usuarioId, Guid tenantId)
        {
            tenantId = GetCurrentTenantId(tenantId);
            var usuario = await _usuarioRepository.GetAsync(usuarioId);
            if (usuario == null || usuario.TenantId != tenantId)
            {
                _logger.LogWarning("ClasificarUsuarioAsync: Usuario no encontrado (ID: {UsuarioId}) o no pertenece al Tenant (ID: {TenantId}).", usuarioId, tenantId);
                return null; // Representa "sin segmento" o el default implícito
            }

            var audienciasDefinidas = (await _audienciaRepository.ListByTenantIdWithReglasAsync(tenantId, soloActivas: true, ordenarPorPrioridad: true))
                                      .ToList();

            if (!audienciasDefinidas.Any())
            {
                _logger.LogInformation("ClasificarUsuarioAsync: No hay audiencias activas para TenantId {TenantId} para UsuarioId {UsuarioId}.", tenantId, usuarioId);
                return null;
            }

            var datosTransacciones = _transaccionRepository != null
                ? await _ObtenerDatosTransaccionesUsuarioAsync(usuario.Id)
                : new DatosTransaccionesUsuario();

            Guid? segmentoGuid = await _ruleEngine.ClasificarUsuarioAsync(usuario, audienciasDefinidas, datosTransacciones);

            if (segmentoGuid.HasValue)
            {
                var audienciaAsignada = audienciasDefinidas.FirstOrDefault(a => a.Id == segmentoGuid.Value); // audienciasDefinidas ya está en memoria
                return audienciaAsignada?.NombreUnicoInterno; // Devolver el string NombreUnicoInterno
            }
            return null; // Sin segmento asignado
        }

        public async Task<IEnumerable<Usuario>> GetUsuariosPorAudienciaAsync(Guid tenantId, string nombreUnicoAudiencia)
        {
            tenantId = GetCurrentTenantId(tenantId);
            if (string.IsNullOrWhiteSpace(nombreUnicoAudiencia))
            {
                // Devolver usuarios con SegmentoDinamicoId == null (no asignados)
                var todosUsuarios = await _usuarioRepository.GetAllByTenantAsync(tenantId);
                return todosUsuarios.Where(u => u.Rol == RolUsuario.UsuarioFinal && !u.SegmentoDinamicoId.HasValue);
            }

            var audienciaObjetivo = await _audienciaRepository.GetByNombreUnicoWithReglasAsync(tenantId, nombreUnicoAudiencia);
            if (audienciaObjetivo == null)
            {
                _logger.LogWarning("GetUsuariosPorAudienciaAsync: Audiencia con NombreUnico '{Nombre}' no encontrada para TenantId {TenantId}.", nombreUnicoAudiencia, tenantId);
                return Enumerable.Empty<Usuario>();
            }

            var usuariosDelTenant = await _usuarioRepository.GetAllByTenantAsync(tenantId);
            return usuariosDelTenant.Where(u => u.Rol == RolUsuario.UsuarioFinal && u.SegmentoDinamicoId == audienciaObjetivo.Id);
        }

        public async Task<Dictionary<string, IEnumerable<Usuario>>> GetAllAudienciasConUsuariosAsync(Guid tenantId)
        {
            tenantId = GetCurrentTenantId(tenantId);
            _logger.LogDebug("Iniciando GetTodasLasAudienciasConUsuariosAsync para TenantId: {TenantId}", tenantId);
            await ActualizarSegmentosUsuariosAsync(tenantId, null);

            var todosLosUsuariosDelTenant = await _usuarioRepository.GetAllByTenantAsync(tenantId);
            var usuariosFinales = todosLosUsuariosDelTenant.Where(u => u.Rol == RolUsuario.UsuarioFinal);

            var audienciasDefinidasMap = (await _audienciaRepository.ListByTenantIdWithReglasAsync(tenantId, soloActivas: true))
                                         .ToDictionary(a => a.Id); // Guid -> Audiencia

            var resultadoAgrupado = new Dictionary<string, List<Usuario>>();

            // Inicializar con todas las audiencias activas definidas
            foreach (var audDef in audienciasDefinidasMap.Values)
            {
                resultadoAgrupado[audDef.NombreUnicoInterno] = new List<Usuario>();
            }
            // Añadir entrada para usuarios no asignados
            if (!resultadoAgrupado.ContainsKey(KeyParaUsuariosNoAsignados))
            {
                resultadoAgrupado[KeyParaUsuariosNoAsignados] = new List<Usuario>();
            }

            foreach (var usuario in usuariosFinales)
            {
                if (usuario.SegmentoDinamicoId.HasValue && audienciasDefinidasMap.TryGetValue(usuario.SegmentoDinamicoId.Value, out var audDef))
                {
                    resultadoAgrupado[audDef.NombreUnicoInterno].Add(usuario);
                }
                else
                {
                    resultadoAgrupado[KeyParaUsuariosNoAsignados].Add(usuario);
                }
            }
            return resultadoAgrupado.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.AsEnumerable());
        }

        public async Task<Dictionary<string, int>> GetDistribucionUsuariosPorAudienciaAsync(Guid tenantId)
        {
            tenantId = GetCurrentTenantId(tenantId);
            var audienciasConUsuarios = await GetAllAudienciasConUsuariosAsync(tenantId); // Ya actualiza y agrupa
            return audienciasConUsuarios.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Count());
        }

        public async Task<EstadisticasAudienciaDinamica> GetEstadisticasGlobalesYAporAudienciaAsync(Guid tenantId)
        {
            tenantId = GetCurrentTenantId(tenantId);
            var resultado = new EstadisticasAudienciaDinamica { TenantId = tenantId };

            // Obtener nombres para los IDs de las audiencias
            var audienciasDefinidas = (await _audienciaRepository.ListByTenantIdWithReglasAsync(tenantId, soloActivas: true)).ToList();

            var audienciasConUsuarios = await GetAllAudienciasConUsuariosAsync(tenantId);

            foreach (var kvp in audienciasConUsuarios)
            {
                string nombreUnicoAudienciaKey = kvp.Key; // NombreUnicoInterno o KeyParaUsuariosNoAsignados
                List<Usuario> usuariosDeLaAudiencia = kvp.Value.ToList();

                var definicionAudienciaActual = audienciasDefinidas.FirstOrDefault(a => a.NombreUnicoInterno == nombreUnicoAudienciaKey);
                string nombreAudienciaAmigable = definicionAudienciaActual?.NombreDescriptivo ??
                                               (nombreUnicoAudienciaKey == KeyParaUsuariosNoAsignados ? "Usuarios Sin Asignar" : $"ID: {nombreUnicoAudienciaKey}");
                // ... (resto de la lógica de estadísticas como antes) ...
                var estSegmento = new EstadisticasDeUnSegmento { /* ... */ NombreSegmento = nombreAudienciaAmigable, SegmentoId = nombreUnicoAudienciaKey };
                estSegmento.TotalUsuarios = usuariosDeLaAudiencia.Count;
                // ...
                resultado.EstadisticasPorSegmento[nombreUnicoAudienciaKey] = estSegmento;
                resultado.TotalUsuariosGeneral += estSegmento.TotalUsuarios;
            }
            // ... (calcular promedios generales) ...
            return resultado;
        }

        public async Task<IEnumerable<Audiencia>> GetDefinicionesAudienciaAsync(Guid tenantId)
        {
            tenantId = GetCurrentTenantId(tenantId);
            return await _audienciaRepository.ListByTenantIdWithReglasAsync(tenantId, soloActivas: false, ordenarPorPrioridad: true); // Devolver todas, no solo activas
        }

        // Método privado para obtener datos de transacciones
        private async Task<DatosTransaccionesUsuario> _ObtenerDatosTransaccionesUsuarioAsync(Guid usuarioId)
        {
            if (_transaccionRepository == null)
            {
                _logger.LogDebug("_ObtenerDatosTransaccionesUsuarioAsync: ITransaccionRepository no inyectado. Devolviendo datos vacíos para UsuarioId: {UsuarioId}", usuarioId);
                return new DatosTransaccionesUsuario();
            }
            try
            {
                // Ejemplo: return await _transaccionRepository.GetAggregatedDataByUserIdAsync(usuarioId);
                _logger.LogTrace("_ObtenerDatosTransaccionesUsuarioAsync: Placeholder para UsuarioId {UsuarioId}. Implementar con _transaccionRepository.", usuarioId);
                return await Task.FromResult(new DatosTransaccionesUsuario());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "_ObtenerDatosTransaccionesUsuarioAsync: Error para UsuarioId: {UsuarioId}", usuarioId);
                return new DatosTransaccionesUsuario();
            }
        }
    }
}