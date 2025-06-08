using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiPuntos.Core.Entities
{
    public class ProductoCanjeable
    {
        public Guid Id { get; set; }
        required public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int CostoEnPuntos { get; set; }

        // Puede estar disponible globalmente o por ubicación
        public List<ProductoUbicacion>? DisponibilidadesPorUbicacion { get; set; }

        //Constructor
        public ProductoCanjeable() { }

        [SetsRequiredMembers]
        public ProductoCanjeable(string nombre, string? descripcion, int costoEnPuntos)
        {
            Nombre = nombre;
            Descripcion = descripcion;
            CostoEnPuntos = costoEnPuntos;
        }

    }
}
