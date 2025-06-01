namespace ServiPuntos.Core.DTOs
{
    /// <summary>
    /// Contiene estadísticas específicas para un único segmento/audiencia dinámica.
    /// </summary>
    public class EstadisticasDeUnSegmento
    {
        /// <summary>
        /// El identificador único del segmento/audiencia (corresponde a Audiencia.NombreUnicoInterno).
        /// </summary>
        public string SegmentoId { get; set; }

        /// <summary>
        /// El nombre descriptivo y legible del segmento/audiencia (corresponde a Audiencia.NombreDescriptivo).
        /// </summary>
        public string NombreSegmento { get; set; }

        public int TotalUsuarios { get; set; }
        public decimal GastoTotal { get; set; }
        public decimal GastoPromedio { get; set; }
        public int VisitasTotales { get; set; }
        public decimal VisitasPromedio { get; set; } // Podría ser decimal

        // Podrías añadir más métricas específicas del segmento aquí si es necesario
        // Ejemplo: Tasa de conversión, productos más comprados por este segmento, etc.
    }
}
