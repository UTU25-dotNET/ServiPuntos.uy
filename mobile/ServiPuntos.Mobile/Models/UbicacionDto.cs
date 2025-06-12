namespace ServiPuntos.Mobile.Models
{
    public class UbicacionDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Nombre { get; set; } = "";
        public string Direccion { get; set; } = "";
        public string Ciudad { get; set; } = "";
        public string Departamento { get; set; } = "";
        public string? Telefono { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public TimeSpan HoraApertura { get; set; }
        public TimeSpan HoraCierre { get; set; }
        public bool LavadoDeAuto { get; set; }
        public bool Lavado { get; set; }
        public bool CambioDeAceite { get; set; }
        public bool CambioDeNeumaticos { get; set; }
        public decimal PrecioNaftaSuper { get; set; }
        public decimal PrecioNaftaPremium { get; set; }
        public decimal PrecioDiesel { get; set; }
        public object[] ProductosLocales { get; set; } = Array.Empty<object>();
        public object[] Promociones { get; set; } = Array.Empty<object>();

        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
    }
}
