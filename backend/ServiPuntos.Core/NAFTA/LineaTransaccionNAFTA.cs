namespace ServiPuntos.Core.NAFTA
{
    public class LineaTransaccionNAFTA
    {
        public Guid IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public string Categoria { get; set; }
        public int Cantidad { get; set; }
        public int PrecioUnitario { get; set; }
        public int SubTotal { get; set; }
    }
}