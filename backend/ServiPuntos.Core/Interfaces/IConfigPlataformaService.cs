using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiPuntos.Core.Entities;

namespace ServiPuntos.Core.Interfaces
{
    public interface IConfigPlataformaService
    {
        Task<ConfigPlataforma?> ObtenerConfiguracionAsync();
        Task ActualizarConfiguracionAsync(ConfigPlataforma config);
    }
}
