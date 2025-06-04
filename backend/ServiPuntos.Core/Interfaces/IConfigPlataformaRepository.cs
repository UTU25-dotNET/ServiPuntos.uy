using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiPuntos.Core.Entities;

namespace ServiPuntos.Core.Interfaces
{
    public interface IConfigPlataformaRepository
    {
        Task<ConfigPlataforma?> ObtenerAsync();
        Task ActualizarAsync(ConfigPlataforma config);

    }
}
