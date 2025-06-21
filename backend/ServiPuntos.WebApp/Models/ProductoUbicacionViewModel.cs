using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ServiPuntos.Core.Entities;

namespace ServiPuntos.WebApp.Models
{
    // ViewModels para ProductoUbicacion
    public class ProductoUbicacionIndexViewModel
    {
        public List<ProductoUbicacion> ProductosUbicacion { get; set; } = new();
        public List<Ubicacion> Ubicaciones { get; set; } = new();
    }

    public class AsignarProductoUbicacionViewModel
    {
        public List<ProductoSelectionViewModel> Productos { get; set; } = new();
        public List<UbicacionSelectionViewModel> Ubicaciones { get; set; } = new();

        [Range(0, double.MaxValue, ErrorMessage = "El precio no puede ser negativo")]
        public double Precio { get; set; }

        [Required(ErrorMessage = "El stock inicial es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock inicial no puede ser negativo")]
        public int StockInicial { get; set; } = 10;
    }

    public class ProductoSelectionViewModel
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int CostoEnPuntos { get; set; }
        public string? FotoUrl { get; set; }
        public bool Selected { get; set; }
    }

    public class UbicacionSelectionViewModel
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Selected { get; set; }
    }

    public class GestionarStockViewModel
    {
        public Guid UbicacionId { get; set; }
        public string UbicacionNombre { get; set; } = string.Empty;
        public List<ProductoUbicacion> ProductosUbicacion { get; set; } = new();
    }

    public class EditProductoUbicacionViewModel
    {
        public Guid Id { get; set; }
        public Guid UbicacionId { get; set; }
        public Guid ProductoCanjeableId { get; set; }

        [Required(ErrorMessage = "El stock disponible es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int StockDisponible { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El precio no puede ser negativo")]
        public double Precio { get; set; }
        public bool Activo { get; set; }
        public string UbicacionNombre { get; set; } = string.Empty;
        public string ProductoNombre { get; set; } = string.Empty;
    }

// ViewModel utilizado al asignar un producto canjeable a múltiples
// ubicaciones desde ProductoCanjeableWApp
public class AsignarUbicacionesProductoViewModel
{
    public Guid ProductoId { get; set; }
    public string ProductoNombre { get; set; } = string.Empty;
    public int CostoEnPuntos { get; set; }
    public List<UbicacionSelectionViewModel> Ubicaciones { get; set; } = new();

    [Required(ErrorMessage = "El stock inicial es obligatorio")]
    [Range(0, int.MaxValue, ErrorMessage = "El stock inicial no puede ser negativo")]
    public int StockInicial { get; set; } = 10;
}

}
