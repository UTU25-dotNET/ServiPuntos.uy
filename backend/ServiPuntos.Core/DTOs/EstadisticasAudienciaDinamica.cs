namespace ServiPuntos.Core.DTOs
{
    /// <summary>
    /// Contiene estadísticas globales de la audiencia de un tenant y un desglose por cada segmento/audiencia dinámica.
    /// </summary>
    public class EstadisticasAudienciaDinamica
    {
        public Guid TenantId { get; set; }

        // --- Métricas Globales (sumando todos los segmentos/audiencias) ---
        public int TotalUsuariosGeneral { get; set; }
        public decimal GastoTotalGeneral { get; set; }
        public decimal GastoPromedioGeneral { get; set; }
        public int VisitasTotalesGeneral { get; set; }
        public decimal VisitasPromedioGeneral { get; set; } // Podría ser decimal para más precisión

        /// <summary>
        /// Diccionario donde la clave es el NombreUnicoInterno de la Audiencia
        /// y el valor son las estadísticas específicas para esa audiencia.
        /// </summary>
        public Dictionary<string, EstadisticasDeUnSegmento> EstadisticasPorSegmento { get; set; }

        public EstadisticasAudienciaDinamica()
        {
            EstadisticasPorSegmento = new Dictionary<string, EstadisticasDeUnSegmento>();
        }
    }
}
