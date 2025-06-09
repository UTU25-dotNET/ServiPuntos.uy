using System;

namespace ServiPuntos.Mobile.Models
{
    public class TransactionSummary
    {
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }
        public int Puntos { get; set; }
        public string Tipo { get; set; }
    }
}
