namespace ServiPuntos.Core.Entities
{
    // Entidad de unión para Promociones y ProductosCanjeables
    public class PromocionProducto
    {
        public Guid PromocionId { get; set; }
        public Promocion? Promocion { get; set; }

        public Guid ProductoCanjeableId { get; set; }
        public ProductoCanjeable? ProductoCanjeable { get; set; }
    }
}