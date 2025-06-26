using System;

namespace ServiPuntos.Mobile.Models
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public Guid ProductoCanjeableId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public int StockDisponible { get; set; }           // (Opcional) 
        public bool Activo { get; set; }                   // (Opcional)
    }

}
