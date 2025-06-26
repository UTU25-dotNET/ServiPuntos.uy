using System;
using System.Collections.Generic;

namespace ServiPuntos.Mobile.Models
{
    public class OfferDto
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Tipo { get; set; }
        public int? PrecioEnPuntos { get; set; }
        public decimal? PrecioEnPesos { get; set; }
        public decimal? DescuentoEnPesos { get; set; }
        public List<Guid> Ubicaciones { get; set; }
        public List<Guid> ProductoIds { get; set; }
    }
}
