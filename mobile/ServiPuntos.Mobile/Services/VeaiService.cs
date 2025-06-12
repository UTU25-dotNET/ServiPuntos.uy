using System;
using System.Threading.Tasks;
using Refit;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    // Refit API interface para verify endpoints
    public interface IVeaiApi
    {
        // GET api/verify/age_verify?cedula={cedula}
        [Get("/age_verify")]
        Task<VeaiResponse> VerifyAgeAsync([AliasAs("cedula")] string cedula);
    }

    // Servicio y su interfaz combinados para VEAI
    public interface IVeaiService
    {
        Task<VeaiResponse?> VerifyAgeAsync(string cedula);
    }

    public class VeaiService : IVeaiService
    {
        private readonly IVeaiApi _api;

        public VeaiService(IVeaiApi api)
        {
            _api = api;
        }

        public async Task<VeaiResponse?> VerifyAgeAsync(string cedula)
        {
            try
            {
                return await _api.VerifyAgeAsync(cedula);
            }
            catch (ApiException)
            {
                return null;
            }
        }
    }
}
