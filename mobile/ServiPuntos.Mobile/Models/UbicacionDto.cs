using System;

namespace ServiPuntos.Mobile.Models
{
    public class UbicacionDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public Guid TenantId { get; set; }
    }
}
