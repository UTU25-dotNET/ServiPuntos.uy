namespace ServiPuntos.Core.NAFTA
{
    public class LineaTransaccionNAFTA
    {
        public string IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public string Categoria { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal SubTotal { get; set; }
    }
}