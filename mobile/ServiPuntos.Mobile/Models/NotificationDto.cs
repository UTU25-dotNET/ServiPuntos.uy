using System;

namespace ServiPuntos.Mobile.Models
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
        public bool Leida { get; set; }
    }
}
