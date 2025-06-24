using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace ServiPuntos.Mobile.Handlers
{
    public class AuthMessageHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var jwt = await SecureStorage.GetAsync("jwt_token");
            if (!string.IsNullOrEmpty(jwt))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", jwt);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
