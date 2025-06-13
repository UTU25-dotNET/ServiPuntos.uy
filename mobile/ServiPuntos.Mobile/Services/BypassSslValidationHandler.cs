using System.Net.Http;

namespace ServiPuntos.Mobile.Services
{
    public class BypassSslValidationHandler : HttpClientHandler
    {
        public BypassSslValidationHandler()
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
        }
    }
}
