// Namespace: ServiPuntos.Application.Services.Rules
// Propósito: Implementación del motor de reglas para la segmentación de audiencias.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
<<<<<<< HEAD
=======
using System.Text.RegularExpressions;
>>>>>>> origin/dev
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
<<<<<<< HEAD
                { "GastoTotal", usuario.GastoTotal }, { "TotalVisitas", usuario.TotalVisitas },
                { "EsSubscriptorPremium", usuario.EsSubscriptorPremium }, { "CiudadResidencia", usuario.CiudadResidencia },
                { "Intereses", usuario.Intereses }, { "UltimaCategoriaComprada", usuario.UltimaCategoriaComprada },
                { "FechaCreacionUsuario", usuario.FechaCreacion }, { "FechaUltimaCompra", usuario.UltimaVisita},
                { "VerificadoVEAI", usuario.VerificadoVEAI },
                { "TotalPuntosGanados", datosTransacciones.TotalPuntosGanados },
                { "NumeroTotalTransacciones", datosTransacciones.TotalTransacciones },
                { "MontoTotalCompras", datosTransacciones.MontoTotal }
=======
                { "GastoTotal", usuario.GastoTotal },
                { "TotalVisitas", usuario.TotalVisitas },
                { "EsSubscriptorPremium", usuario.EsSubscriptorPremium },
                { "CiudadResidencia", usuario.CiudadResidencia },
                { "Intereses", usuario.Intereses },
                { "UltimaCategoriaComprada", usuario.UltimaCategoriaComprada },
                { "FechaCreacionUsuario", usuario.FechaCreacion },
                { "FechaUltimaCompra", usuario.UltimaVisita },
                { "VerificadoVEAI", usuario.VerificadoVEAI },
                { "TotalPuntosGanados", datosTransacciones.TotalPuntosGanados },
                { "NumeroTotalTransacciones", datosTransacciones.TotalTransacciones },
                { "MontoTotalCompras", datosTransacciones.MontoTotal },
                { "Puntos", usuario.Puntos },
                { "Email", usuario.Email },
                { "Nombre", usuario.Nombre },
                { "Apellido", usuario.Apellido },
                { "Telefono", usuario.Telefono },
                { "FechaCreacion", usuario.FechaCreacion },
                { "FechaModificacion", usuario.FechaModificacion }
>>>>>>> origin/dev
            };
            datos["DiasDesdeRegistro"] = (ahora - usuario.FechaCreacion).Days;
            if (usuario.FechaNacimiento.HasValue) { int edad = ahora.Year - usuario.FechaNacimiento.Value.Year; if (usuario.FechaNacimiento.Value.Date > ahora.AddYears(-edad)) edad--; datos["Edad"] = edad; } else { datos["Edad"] = null; }
            if (usuario.UltimaVisita.HasValue) { datos["DiasSinVisita"] = (ahora - usuario.UltimaVisita.Value).Days; } else { datos["DiasSinVisita"] = null; }
            _logger.LogTrace("RuleEngine: Datos preparados para UsuarioId {UsuarioId}: {Keys}", usuario.Id, string.Join(",", datos.Keys));
            return datos;
        }

        private bool EvaluarReglaIndividual(Dictionary<string, object> datosDelUsuario, ReglaAudiencia regla)
        {
<<<<<<< HEAD

            if (!datosDelUsuario.TryGetValue(regla.Propiedad, out var valorDelUsuarioObj)) { /* ... */ return false; }
            if (valorDelUsuarioObj == null) { /* ... */ return false; }
            string operadorNormalizado = NormalizarOperador(regla.Operador);
            try { /* ... toda la lógica de switch por tipo y operador ... */ return false; } // Placeholder, copia la lógica completa
            catch (Exception ex) { _logger.LogError(ex, "RuleEngine: Error _EvaluarReglaIndividual"); return false; }
=======
            if (!datosDelUsuario.TryGetValue(regla.Propiedad, out var valorDelUsuarioObj))
            {
                _logger.LogTrace("Propiedad {Prop} no encontrada en datos del usuario", regla.Propiedad);
                return false;
            }
            if (valorDelUsuarioObj == null)
            {
                return false;
            }

            string operador = NormalizarOperador(regla.Operador);
            string valorComparacion = regla.Valor ?? string.Empty;

            try
            {
                switch (operador)
                {
                    case "is_null_or_empty":
                        return string.IsNullOrEmpty(valorDelUsuarioObj.ToString());
                    case "is_not_null_or_empty":
                        return !string.IsNullOrEmpty(valorDelUsuarioObj.ToString());
                }

                // Manejo de listas para CONTAINS, IN, NOT_IN
                if (operador == "contains")
                {
                    if (valorDelUsuarioObj is IEnumerable<string> lista)
                        return lista.Any(e => string.Equals(e, valorComparacion, StringComparison.OrdinalIgnoreCase));
                    return valorDelUsuarioObj.ToString()!.IndexOf(valorComparacion, StringComparison.OrdinalIgnoreCase) >= 0;
                }
                if (operador == "starts_with")
                    return valorDelUsuarioObj.ToString()!.StartsWith(valorComparacion, StringComparison.OrdinalIgnoreCase);
                if (operador == "ends_with")
                    return valorDelUsuarioObj.ToString()!.EndsWith(valorComparacion, StringComparison.OrdinalIgnoreCase);
                if (operador == "regex")
                    return Regex.IsMatch(valorDelUsuarioObj.ToString()!, valorComparacion);

                // IN / NOT_IN
                if (operador == "in" || operador == "not_in")
                {
                    var listaValores = valorComparacion.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                        .Select(v => v.Trim())
                                                        .ToList();
                    bool contiene;
                    if (valorDelUsuarioObj is IEnumerable<string> listaUsr)
                    {
                        contiene = listaUsr.Any(v => listaValores.Contains(v, StringComparer.OrdinalIgnoreCase));
                    }
                    else
                    {
                        contiene = listaValores.Contains(valorDelUsuarioObj.ToString()!, StringComparer.OrdinalIgnoreCase);
                    }
                    return operador == "in" ? contiene : !contiene;
                }

                // Comparaciones numéricas o fecha
                if (TryParseDecimal(valorComparacion, out decimal comparacionDecimal) && decimal.TryParse(valorDelUsuarioObj.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valorDecimal))
                {
                    return operador switch
                    {
                        ">" => valorDecimal > comparacionDecimal,
                        "<" => valorDecimal < comparacionDecimal,
                        ">=" => valorDecimal >= comparacionDecimal,
                        "<=" => valorDecimal <= comparacionDecimal,
                        "=" => valorDecimal == comparacionDecimal,
                        "!=" => valorDecimal != comparacionDecimal,
                        _ => false
                    };
                }
                if (DateTime.TryParse(valorComparacion, out DateTime fechaComp) && DateTime.TryParse(valorDelUsuarioObj.ToString(), out DateTime fechaValor))
                {
                    return operador switch
                    {
                        ">" => fechaValor > fechaComp,
                        "<" => fechaValor < fechaComp,
                        ">=" => fechaValor >= fechaComp,
                        "<=" => fechaValor <= fechaComp,
                        "=" => fechaValor == fechaComp,
                        "!=" => fechaValor != fechaComp,
                        _ => false
                    };
                }
                if (bool.TryParse(valorComparacion, out bool boolComp) && bool.TryParse(valorDelUsuarioObj.ToString(), out bool boolVal))
                {
                    return operador == "=" ? boolVal == boolComp : operador == "!=" && boolVal != boolComp;
                }

                // Fallback a comparación de strings
                return operador switch
                {
                    "=" => string.Equals(valorDelUsuarioObj.ToString(), valorComparacion, StringComparison.OrdinalIgnoreCase),
                    "!=" => !string.Equals(valorDelUsuarioObj.ToString(), valorComparacion, StringComparison.OrdinalIgnoreCase),
                    ">" => string.Compare(valorDelUsuarioObj.ToString(), valorComparacion, StringComparison.OrdinalIgnoreCase) > 0,
                    "<" => string.Compare(valorDelUsuarioObj.ToString(), valorComparacion, StringComparison.OrdinalIgnoreCase) < 0,
                    ">=" => string.Compare(valorDelUsuarioObj.ToString(), valorComparacion, StringComparison.OrdinalIgnoreCase) >= 0,
                    "<=" => string.Compare(valorDelUsuarioObj.ToString(), valorComparacion, StringComparison.OrdinalIgnoreCase) <= 0,
                    _ => false
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RuleEngine: Error EvaluarReglaIndividual");
                return false;
            }
>>>>>>> origin/dev
        }

        private string NormalizarOperador(string operadorInput)
        {
<<<<<<< HEAD
            // ... (COPIA COMPLETA de la lógica de _NormalizarOperador que te di antes) ...
            return operadorInput?.Trim().ToLowerInvariant() switch { "mayorque" => ">", /* ...otros... */ _ => operadorInput?.Trim().ToLowerInvariant() }; // Placeholder
=======
            if (string.IsNullOrWhiteSpace(operadorInput)) return string.Empty;

            return operadorInput.Trim().ToLowerInvariant() switch
            {
                "=" or "==" or "equals" => "=",
                "!=" or "not_equals" => "!=",
                ">" or "greater_than" => ">",
                "<" or "less_than" => "<",
                ">=" or "greater_than_or_equal" => ">=",
                "<=" or "less_than_or_equal" => "<=",
                "contains" => "contains",
                "starts_with" => "starts_with",
                "ends_with" => "ends_with",
                "in" => "in",
                "not_in" => "not_in",
                "is_null_or_empty" => "is_null_or_empty",
                "is_not_null_or_empty" => "is_not_null_or_empty",
                "regex" => "regex",
                _ => operadorInput.Trim().ToLowerInvariant()
            };
>>>>>>> origin/dev
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