using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ServiPuntos.Core.Enums;

namespace ServiPuntos.WebApp.Models
{
    public class CreatePromocionViewModel
    {
        [Required]
        [StringLength(100)]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        public int? PrecioEnPuntos { get; set; }
        public int? DescuentoEnPuntos { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; } = DateTime.UtcNow.Date;

        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; } = DateTime.UtcNow.Date.AddDays(7);

        public TipoPromocion Tipo { get; set; } = TipoPromocion.Promocion;

        public Guid? AudienciaId { get; set; }
        public List<Guid> UbicacionIds { get; set; } = new();
    }

    public class EditPromocionViewModel : CreatePromocionViewModel
    {
        public Guid Id { get; set; }
    }
}