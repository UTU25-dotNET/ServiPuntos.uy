// Models/ProductoUbicacionDto.cs
using System;

namespace ServiPuntos.Mobile.Models
{
    public class ProductoUbicacionDto
    {
        public Guid Id { get; set; }
        public Guid ProductoCanjeableId { get; set; }
        public int StockDisponible { get; set; }
        public string? Nombre { get; set; }      
        public string? Descripcion { get; set; } 
        public decimal? Precio { get; set; }     

    }
}
