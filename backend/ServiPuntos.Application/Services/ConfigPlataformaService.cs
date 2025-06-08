using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Core.Entities;

namespace ServiPuntos.Application.Services
{
    public class ConfigPlataformaService : IConfigPlataformaService
    {
        private readonly IConfigPlataformaRepository _iConfigPlataformaRepository;

        public ConfigPlataformaService(IConfigPlataformaRepository configPlataformaRepository)
        {
            _iConfigPlataformaRepository = configPlataformaRepository;
        }
        public async Task<ConfigPlataforma?> ObtenerConfiguracionAsync()
        {
            return await _iConfigPlataformaRepository.ObtenerAsync();
        }

        public async Task ActualizarConfiguracionAsync(ConfigPlataforma config)
        {
            await _iConfigPlataformaRepository.ActualizarAsync(config);
        }
    }
}
