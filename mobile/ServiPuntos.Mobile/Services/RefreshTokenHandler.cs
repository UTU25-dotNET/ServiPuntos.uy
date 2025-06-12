using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace ServiPuntos.Mobile.Services
{
    /// <summary>
    /// DelegatingHandler que intercepta respuestas 401 y renueva el token automáticamente.
    /// </summary>
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
            // Primera llamada
            var response = await base.SendAsync(request, cancellationToken);

            // Si viene 401 Unauthenticated, intentamos renovar
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refreshed = await _authService.RefreshTokenAsync();
                if (refreshed)
                {
                    // Recuperar nuevo token
                    var newToken = await SecureStorage.GetAsync("auth_token");
                    if (!string.IsNullOrEmpty(newToken))
                    {
                        request.Headers.Authorization =
                            new AuthenticationHeaderValue("Bearer", newToken);
                        // Reintentar la llamada original
                        response = await base.SendAsync(request, cancellationToken);
                    }
                }
            }

            return response;
        }
    }
}
