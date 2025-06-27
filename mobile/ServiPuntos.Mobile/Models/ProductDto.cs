using System;

namespace ServiPuntos.Mobile.Models
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public Guid ProductoCanjeableId { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
    }

}
