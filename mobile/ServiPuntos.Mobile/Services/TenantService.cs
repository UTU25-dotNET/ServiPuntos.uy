using ServiPuntos.Mobile.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiPuntos.Mobile.Services
{
    public class TenantService
    {
        // En producción, esto iría a un API
        public async Task<List<TenantConfig>> GetTenantsAsync()
        {
            await Task.Delay(500); // Simula latencia de red
            return new List<TenantConfig>
            {
                new TenantConfig {
                    Id = "ancap", Name = "ANCAP",
                    LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/8/8b/Logo_Ancap.svg",
                    PrimaryColor = "#ffdd00", SecondaryColor = "#232d4b"
                },
                new TenantConfig {
                    Id = "axion", Name = "AXION",
                    LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/f/fc/AXION_logo.png",
                    PrimaryColor = "#ec008c", SecondaryColor = "#282828"
                }
            };
        }
    }
}
