using System.Diagnostics.CodeAnalysis;

namespace ServiPuntos.Core.Entities
{
    public class Ubicacion
    {
        public Guid Id { get; set; }

        public required Guid TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        public string? Nombre { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
        public string? Departamento { get; set; }
        public string? Telefono { get; set; }

        public double Latitud { get; set; }
        public double Longitud { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }

        public TimeSpan HoraApertura { get; set; }
        public TimeSpan HoraCierre { get; set; }

        public bool Lavado { get; set; }

        public bool LavadoDeAuto { get; set; }
        public bool CambioDeAceite { get; set; }
        public bool CambioDeNeumaticos { get; set; }

        public decimal? PrecioLavado { get; set; }
        public decimal? PrecioCambioAceite { get; set; }
        public decimal? PrecioCambioNeumaticos { get; set; }

        public decimal? PrecioNaftaSuper { get; set; }
        public decimal? PrecioNaftaPremium { get; set; }
        public decimal? PrecioDiesel { get; set; }

        public List<ProductoUbicacion> ProductosLocales { get; set; } = new List<ProductoUbicacion>();
        public List<Promocion> Promociones { get; set; } = new List<Promocion>();

        //Constructor
        public Ubicacion() { }
        [SetsRequiredMembers]
        public Ubicacion(Guid tenantId, string nombre, string direccion, string ciudad, string departamento, string telefono,
            TimeSpan horaApertura, TimeSpan horaCierre, double latitud, double longitud)
        {
            TenantId = tenantId;
            Nombre = nombre;
            Direccion = direccion;
            Ciudad = ciudad;
            Departamento = departamento;
            Telefono = telefono;
            HoraApertura = horaApertura;
            HoraCierre = horaCierre;
            Latitud = latitud;
            Longitud = longitud;
        }
    }
}
