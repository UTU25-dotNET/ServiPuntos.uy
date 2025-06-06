using System.ComponentModel.DataAnnotations;
using ServiPuntos.Core.Entities;

namespace ServiPuntos.WebApp.Models
{
    public class ProductosCanjeablesIndexViewModel
    {
        public List<ProductoCanjeable> Productos { get; set; } = new();
        public List<Ubicacion> Ubicaciones { get; set; } = new();
    }
    public class CreateProductoCanjeableViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;
        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string? Descripcion { get; set; }
        [Required(ErrorMessage = "El costo en puntos es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "El costo en puntos debe ser mayor a 0")]
        public int CostoEnPuntos { get; set; }
    }
    public class EditProductoCanjeableViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;
        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string? Descripcion { get; set; }
        [Required(ErrorMessage = "El costo en puntos es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "El costo en puntos debe ser mayor a 0")]
        public int CostoEnPuntos { get; set; }
    }
    public class AsignarProductoUbicacionViewModel
    {
        public Guid ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public List<UbicacionSelectionViewModel> Ubicaciones { get; set; } = new();
        [Required(ErrorMessage = "El stock inicial es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock inicial debe ser mayor o igual a 0")]
        public int StockInicial { get; set; }
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
}
