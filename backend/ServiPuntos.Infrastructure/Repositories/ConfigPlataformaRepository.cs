using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServiPuntos.Core.Entities;
using ServiPuntos.Infrastructure.Data;
using ServiPuntos.Core.Interfaces;

namespace ServiPuntos.Infrastructure.Repositories
{
    public class ConfigPlataformaRepository : IConfigPlataformaRepository
    {
        private readonly ServiPuntosDbContext _dbContext;

        public ConfigPlataformaRepository(ServiPuntosDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ConfigPlataforma?> ObtenerAsync()
        {
            return await _dbContext.ConfigPlataformas.FirstOrDefaultAsync();
        }

        public async Task ActualizarAsync(ConfigPlataforma config)
        {
            var existente = await _dbContext.ConfigPlataformas.FirstOrDefaultAsync();
            if (existente == null)
            {
                _dbContext.ConfigPlataformas.Add(config);
            }
            else
            {
                existente.MaximoIntentosLogin = config.MaximoIntentosLogin;
                existente.TiempoExpiracion = config.TiempoExpiracion;
                existente.LargoMinimoPassword = config.LargoMinimoPassword;
            }

            await _dbContext.SaveChangesAsync();
        }
    }


}
