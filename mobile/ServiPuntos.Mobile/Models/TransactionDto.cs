namespace ServiPuntos.Mobile.Models
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public DateTime Fecha { get; set; }
        public string TipoTransaccion { get; set; } = "";
        public decimal Monto { get; set; }
        public int PuntosAsignados { get; set; }
        // Opcional: lista de productos o servicios involucrados
        public object[] Detalles { get; set; } = Array.Empty<object>();
    }
}
