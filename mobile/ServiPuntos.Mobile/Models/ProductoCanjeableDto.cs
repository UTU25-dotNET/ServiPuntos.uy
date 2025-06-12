namespace ServiPuntos.Mobile.Models
{
    public class ProductoCanjeableDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = "";
        public string? Descripcion { get; set; }
        public int CostoEnPuntos { get; set; }
        public string? FotoUrl { get; set; }
        // Para el catálogo, enlazamos la disponiblidad:
        public int StockDisponible { get; set; }
    }
}
