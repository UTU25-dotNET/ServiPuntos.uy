using System;

namespace ServiPuntos.Mobile.Models
{
    public class CanjeHistorialItem
    {
        public string Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Producto { get; set; }
        public string Ubicacion { get; set; }
        public int Puntos { get; set; }
        public string Estado { get; set; }
    }
}
