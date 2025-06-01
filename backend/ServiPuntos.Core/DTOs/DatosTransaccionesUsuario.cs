namespace ServiPuntos.Core.DTOs
{
    public class DatosTransaccionesUsuario
    {
        /// <summary>
        /// Total de puntos de lealtad que el usuario ha ganado a lo largo del tiempo.
        /// </summary>
        public int TotalPuntosGanados { get; set; } = 0;

        /// <summary>
        /// Número total de transacciones (compras, interacciones, etc.) que el usuario ha realizado.
        /// </summary>
        public int TotalTransacciones { get; set; } = 0;

        /// <summary>
        /// Suma total del monto de todas las transacciones del usuario.
        /// </summary>
        public decimal MontoTotal { get; set; } = 0m;

        // Puedes añadir más agregados aquí según tus necesidades de segmentación, por ejemplo:
        // public decimal GastoPromedioPorTransaccion { get; set; }
        // public DateTime? FechaUltimaTransaccionConPuntos { get; set; }
        // public int CantidadProductosDistintosComprados { get; set; }

        // Constructor por defecto para inicializar con valores seguros
        public DatosTransaccionesUsuario()
        {
        }
        public DatosTransaccionesUsuario(int totalPuntosGanados, int totalTransacciones, decimal montoTotal)
        {
            TotalPuntosGanados = totalPuntosGanados;
            TotalTransacciones = totalTransacciones;
            MontoTotal = montoTotal;
        }
    }
}
