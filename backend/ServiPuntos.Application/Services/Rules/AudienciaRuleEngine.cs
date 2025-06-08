// Namespace: ServiPuntos.Application.Services.Rules
// Propósito: Implementación del motor de reglas para la segmentación de audiencias.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ServiPuntos.Core.DTOs;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces; // Para IAudienciaRuleEngine y DatosTransaccionesUsuario

namespace ServiPuntos.Application.Services.Rules
{
    public class AudienciaRuleEngine : IAudienciaRuleEngine
    {
        private readonly ILogger<AudienciaRuleEngine> _logger;
        // Si el RuleEngine necesitara obtener datos por sí mismo (menos ideal), inyectaría repositorios.
        // Pero es mejor que AudienciaService le pase los datos necesarios.

        //private const string SegmentoPorDefectoId = "comun";

        public AudienciaRuleEngine(ILogger<AudienciaRuleEngine> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid?> ClasificarUsuarioAsync(Usuario usuario,IEnumerable<Audiencia> audienciasDefinidas,DatosTransaccionesUsuario datosTransacciones)
        {
            if (usuario == null) { _logger.LogWarning("RuleEngine: Usuario nulo."); return null; } // Devuelve null
            if (audienciasDefinidas == null || !audienciasDefinidas.Any())
            {
                _logger.LogInformation("RuleEngine: No hay audiencias definidas para UsuarioId {UsuarioId}.", usuario.Id);
                return null; // Devuelve null (o el AudienciaComunConocidaId si lo tienes)
            }

            var datosDelUsuario = PrepararDatosDelUsuarioParaEvaluacion(usuario, datosTransacciones);

            foreach (var audienciaDef in audienciasDefinidas) // Asumimos ordenadas por prioridad y activas
            {
                bool cumpleAudiencia;
                // ... (lógica de evaluación de reglas como antes) ...
                if (audienciaDef.Reglas == null || !audienciaDef.Reglas.Any())
                {
                    cumpleAudiencia = true;
                }
                else
                {
                    var resultadosReglas = new List<bool>();
                    var reglasOrdenadas = audienciaDef.Reglas.OrderBy(r => r.OrdenEvaluacion).ToList();
                    foreach (var regla in reglasOrdenadas)
                    {
                        resultadosReglas.Add(EvaluarReglaIndividual(datosDelUsuario, regla));
                    }
                    cumpleAudiencia = AplicarLogicaSecuencial(resultadosReglas, reglasOrdenadas);
                }


                if (cumpleAudiencia)
                {
                    _logger.LogDebug("UsuarioId {UsuarioId} clasificado en Audiencia (RuleEngine): '{AudId}' ({AudNombre}).",
                        usuario.Id, audienciaDef.Id, audienciaDef.NombreDescriptivo);
                    return audienciaDef.Id; // CAMBIO: Devuelve el Guid de la Audiencia
                }
            }

            _logger.LogInformation("UsuarioId {UsuarioId} no coincidió con ninguna audiencia (RuleEngine).", usuario.Id);
            return null; // CAMBIO: Devuelve null (o el AudienciaComunConocidaId)
        }

        // --- MÉTODOS PRIVADOS DEL MOTOR DE REGLAS ---

        private bool AplicarLogicaSecuencial(List<bool> resultadosReglas, List<ReglaAudiencia> reglasOrdenadas)
        {
            if (!resultadosReglas.Any()) return true;
            bool resultadoAcumulado = resultadosReglas[0];
            for (int i = 0; i < reglasOrdenadas.Count - 1; i++)
            {
                string operadorLogico = reglasOrdenadas[i].OperadorLogicoConSiguiente?.ToUpperInvariant();
                if (operadorLogico == "OR") { resultadoAcumulado = resultadoAcumulado || resultadosReglas[i + 1]; }
                else { resultadoAcumulado = resultadoAcumulado && resultadosReglas[i + 1]; }
            }
            return resultadoAcumulado;
        }

        private Dictionary<string, object> PrepararDatosDelUsuarioParaEvaluacion(Usuario usuario, DatosTransaccionesUsuario datosTransacciones)
        {
            var ahora = DateTime.UtcNow;
            var datos = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                { "GastoTotal", usuario.GastoTotal }, { "TotalVisitas", usuario.TotalVisitas },
                { "EsSubscriptorPremium", usuario.EsSubscriptorPremium }, { "CiudadResidencia", usuario.CiudadResidencia },
                { "Intereses", usuario.Intereses }, { "UltimaCategoriaComprada", usuario.UltimaCategoriaComprada },
                { "FechaCreacionUsuario", usuario.FechaCreacion }, { "FechaUltimaCompra", usuario.UltimaVisita},
                { "VerificadoVEAI", usuario.VerificadoVEAI },
                { "TotalPuntosGanados", datosTransacciones.TotalPuntosGanados },
                { "NumeroTotalTransacciones", datosTransacciones.TotalTransacciones },
                { "MontoTotalCompras", datosTransacciones.MontoTotal }
            };
            datos["DiasDesdeRegistro"] = (ahora - usuario.FechaCreacion).Days;
            if (usuario.FechaNacimiento.HasValue) { int edad = ahora.Year - usuario.FechaNacimiento.Value.Year; if (usuario.FechaNacimiento.Value.Date > ahora.AddYears(-edad)) edad--; datos["Edad"] = edad; } else { datos["Edad"] = null; }
            if (usuario.UltimaVisita.HasValue) { datos["DiasSinVisita"] = (ahora - usuario.UltimaVisita.Value).Days; } else { datos["DiasSinVisita"] = null; }
            _logger.LogTrace("RuleEngine: Datos preparados para UsuarioId {UsuarioId}: {Keys}", usuario.Id, string.Join(",", datos.Keys));
            return datos;
        }

        private bool EvaluarReglaIndividual(Dictionary<string, object> datosDelUsuario, ReglaAudiencia regla)
        {

            if (!datosDelUsuario.TryGetValue(regla.Propiedad, out var valorDelUsuarioObj)) { /* ... */ return false; }
            if (valorDelUsuarioObj == null) { /* ... */ return false; }
            string operadorNormalizado = NormalizarOperador(regla.Operador);
            try { /* ... toda la lógica de switch por tipo y operador ... */ return false; } // Placeholder, copia la lógica completa
            catch (Exception ex) { _logger.LogError(ex, "RuleEngine: Error _EvaluarReglaIndividual"); return false; }
        }

        private string NormalizarOperador(string operadorInput)
        {
            // ... (COPIA COMPLETA de la lógica de _NormalizarOperador que te di antes) ...
            return operadorInput?.Trim().ToLowerInvariant() switch { "mayorque" => ">", /* ...otros... */ _ => operadorInput?.Trim().ToLowerInvariant() }; // Placeholder
        }

        // Los helpers de conversión _TryParseDecimal, etc., también irían aquí si son
        // específicos de la evaluación de reglas, o podrían ser utilidades estáticas en otro lugar.
        private bool TryParseDecimal(string input, out decimal output) // Ejemplo
        {
            return decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out output);
        }
        // ... otros helpers de parseo ...
    }

    // Definición de DatosTransaccionesUsuario (si no está en Core.Entities o Core.Interfaces)
    // Es mejor que esté en un lugar común si el repositorio también la usa.
    // public class DatosTransaccionesUsuario { /* ... */ }
}