using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiPuntos.Core.Entities
{
    public class ConfigPlataforma
    {
        public Guid Id { get; set; }

        public int MaximoIntentosLogin { get; set; } = 3; // Maximo intentos de login antes de bloquear la cuenta
        public int TiempoExpiracion { get; set; }// Tiempo que tarda en deslogear al usuario final
        public int LargoMinimoPassword { get; set; } = 8; // Largo minimo del password

        public ConfigPlataforma() { }
        public ConfigPlataforma(int maximoIntentosLogin, int tiempoExpiracion, int largoMinimoPassword)
        {
            MaximoIntentosLogin = maximoIntentosLogin;
            TiempoExpiracion = tiempoExpiracion;
            LargoMinimoPassword = largoMinimoPassword;
        }

    }
}
