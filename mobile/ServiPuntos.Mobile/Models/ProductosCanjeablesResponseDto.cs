using System.Collections.Generic;

namespace ServiPuntos.Mobile.Models
{
    public class ProductosCanjeablesResponseDto
    {
        public List<CatalogItemDto> Productos { get; set; } = new();
        public List<UbicacionDto> Ubicaciones { get; set; } = new();
    }
}
