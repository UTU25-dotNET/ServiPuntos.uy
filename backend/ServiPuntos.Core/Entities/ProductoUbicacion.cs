using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiPuntos.Core.Entities
{
    // Inventario de un Producto en una Ubicacion
    public class ProductoUbicacion
    {
        public Guid Id { get; set; }
        required public Guid UbicacionId { get; set; }
        public Ubicacion? Ubicacion { get; set; }

        required public Guid ProductoCanjeableId { get; set; }
        public ProductoCanjeable? ProductoCanjeable { get; set; }

        public int StockDisponible { get; set; }
        public bool Activo { get; set; }

        //Constructor
        public ProductoUbicacion() { }
        public ProductoUbicacion(Guid ubicacionId, Guid productoCanjeableId, int stockDisponible)
        {
            UbicacionId = ubicacionId;
            ProductoCanjeableId = productoCanjeableId;
            StockDisponible = stockDisponible;
        }

    }
}
