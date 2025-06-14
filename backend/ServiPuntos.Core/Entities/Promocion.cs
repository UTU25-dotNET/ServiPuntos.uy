namespace ServiPuntos.Core.Entities
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

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public int? DescuentoEnPuntos { get; set; } // opcional

        // Relación con ubicaciones (puede ser global o por estaciones específicas)
        public List<Ubicacion>? Ubicaciones { get; set; }
        public List<PromocionProducto>? Productos { get; set; }

        //Constructor
        public Promocion() { }
        public Promocion(string titulo, string? descripcion, DateTime fechaInicio, DateTime fechaFin, int? descuentoEnPuntos, int? precioEnPuntos, Enums.TipoPromocion tipo, Guid tenantId)
        {
            Titulo = titulo;
            Descripcion = descripcion;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            DescuentoEnPuntos = descuentoEnPuntos;
            PrecioEnPuntos = precioEnPuntos;
            Tipo = tipo;
            TenantId = tenantId;
        }
    }
}
