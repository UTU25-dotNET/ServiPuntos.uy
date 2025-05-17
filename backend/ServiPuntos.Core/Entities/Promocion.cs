using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiPuntos.Core.Entities
{
    public class Promocion
    {
        public Guid Id { get; set; }
        required public string Titulo { get; set; }
        public string? Descripcion { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public int? DescuentoEnPuntos { get; set; } // opcional

        // Relación con ubicaciones (puede ser global o por estaciones específicas)
        public List<Ubicacion>? Ubicaciones { get; set; }

        //Constructor
        public Promocion() { }
        public Promocion(string titulo, string? descripcion, DateTime fechaInicio, DateTime fechaFin, int? descuentoEnPuntos)
        {
            Titulo = titulo;
            Descripcion = descripcion;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            DescuentoEnPuntos = descuentoEnPuntos;
        }
    }
}
