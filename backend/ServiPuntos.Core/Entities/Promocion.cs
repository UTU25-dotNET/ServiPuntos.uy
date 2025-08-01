using System.ComponentModel.DataAnnotations.Schema;
﻿namespace ServiPuntos.Core.Entities
{
    public class Promocion
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid? AudienciaId { get; set; }
        public Audiencia? Audiencia { get; set; }
        public Enums.TipoPromocion Tipo { get; set; } = Enums.TipoPromocion.Promocion;
        required public string Titulo { get; set; }
        public string? Descripcion { get; set; }
        public int? PrecioEnPuntos { get; set; }
        public decimal? PrecioEnPesos { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        [Column("DescuentoEnPuntos")]
        public decimal? DescuentoEnPesos { get; set; } // opcional

        // Relación con ubicaciones (puede ser global o por estaciones específicas)
        public List<Ubicacion>? Ubicaciones { get; set; }
        public List<PromocionProducto>? Productos { get; set; }

        //Constructor
        public Promocion() { }
        public Promocion(string titulo, string? descripcion, DateTime fechaInicio, DateTime fechaFin, decimal? descuentoEnPesos, int? precioEnPuntos, decimal? precioEnPesos, Enums.TipoPromocion tipo, Guid tenantId)
        {
            Titulo = titulo;
            Descripcion = descripcion;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            DescuentoEnPesos = descuentoEnPesos;
            PrecioEnPuntos = precioEnPuntos;
            PrecioEnPesos = precioEnPesos;
            Tipo = tipo;
            TenantId = tenantId;
        }
    }
}
