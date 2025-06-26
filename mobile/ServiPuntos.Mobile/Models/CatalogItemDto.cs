using System;

namespace ServiPuntos.Mobile.Models
{

    public class CatalogItemDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int CostoEnPuntos { get; set; }
        public string FotoUrl { get; set; } = string.Empty;
    }

}
