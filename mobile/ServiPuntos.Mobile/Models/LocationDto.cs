using System;

namespace ServiPuntos.Mobile.Models
{
    public class LocationDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
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
    }
}
