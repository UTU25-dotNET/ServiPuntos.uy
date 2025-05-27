using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ServiPuntos.Core.Entities
{
    public class Ubicacion
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
        public string? Departamento { get; set; }
        public TimeSpan? HoraApertura { get; set; }
        public TimeSpan? HoraCierre { get; set; }
        public bool? Lavado { get; set; }
        public decimal? PrecioNaftaSuper { get; set; }
        public decimal? PrecioNaftaPremium { get; set; }
        public decimal? PrecioDiesel { get; set; }

        public Guid TenantId { get; set; }

        [JsonIgnore]
        public virtual Tenant Tenant { get; set; } = null!;

        // ← Estas dos colecciones son requeridas por tu DbContext:
        public virtual ICollection<ProductoUbicacion> ProductosLocales { get; set; } = new List<ProductoUbicacion>();
        public virtual ICollection<Promocion> Promociones { get; set; } = new List<Promocion>();
    }
}
