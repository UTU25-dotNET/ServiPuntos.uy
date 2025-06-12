using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public interface IVeaiService
    {
        Task<VeaiResponse?> VerifyAgeAsync(string cedula);
    }
}
