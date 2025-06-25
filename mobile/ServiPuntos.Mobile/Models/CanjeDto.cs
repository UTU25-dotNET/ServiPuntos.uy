using System;

namespace ServiPuntos.Mobile.Models
{
    public class CanjeDto
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public Guid ProductoCanjeableId { get; set; }
        public Guid UbicacionId { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public DateTime? FechaExpiracion { get; set; }
        public string Estado { get; set; }
        public int? Puntos { get; set; }
    }
}
