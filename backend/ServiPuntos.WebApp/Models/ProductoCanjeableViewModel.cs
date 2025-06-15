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
        [Url(ErrorMessage = "Debe ingresar una URL válida")]
        public string? FotoUrl { get; set; }
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
        [Url(ErrorMessage = "Debe ingresar una URL válida")]
        public string? FotoUrl { get; set; }
    }
    
}
