using System;

namespace ServiPuntos.WebApp.Models
{
    public class ReporteUbicacionViewModel
    {
        public int TotalTransacciones { get; set; }
        public decimal MontoTotalTransacciones { get; set; }
        public int TotalCanjes { get; set; }
        public int CanjesCompletados { get; set; }
    }
}
