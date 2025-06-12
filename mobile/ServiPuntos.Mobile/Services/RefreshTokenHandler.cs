using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using ServiPuntos.Mobile.Services;

namespace ServiPuntos.Mobile.Services
{
    public class RefreshTokenHandler : DelegatingHandler
    {
        private readonly IAuthService _authService;

        public RefreshTokenHandler(IAuthService authService)
        {
            _authService = authService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {

            var response = await base.SendAsync(request, cancellationToken);


            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var ok = await _authService.RefreshTokenAsync();
                if (ok)
                {

                    var newToken = await SecureStorage.GetAsync("auth_token");
                    request.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", newToken);


                    response = await base.SendAsync(request, cancellationToken);
                }
            }

            return response;
        }
    }
}
