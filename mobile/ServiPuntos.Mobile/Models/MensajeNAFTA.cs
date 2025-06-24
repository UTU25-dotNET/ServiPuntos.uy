using System;
using System.Collections.Generic;

namespace ServiPuntos.Mobile.Models
{
    public class MensajeNAFTA
    {
        public Guid IdMensaje { get; set; } = Guid.NewGuid();
      
        public Dictionary<string, object> Datos { get; set; } = new();
    }
}
