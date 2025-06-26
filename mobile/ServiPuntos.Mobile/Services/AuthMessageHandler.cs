using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using System;

namespace ServiPuntos.Mobile.Handlers
{
    public class AuthMessageHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var jwt = await SecureStorage.GetAsync("auth_token");
            Console.WriteLine($"[AuthHandler] JWT: {jwt}");
            if (!string.IsNullOrEmpty(jwt))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            var tenantId = await SecureStorage.GetAsync("tenant_id");
            Console.WriteLine($"[AuthHandler] TenantId: {tenantId}");
            if (!string.IsNullOrEmpty(tenantId))
                request.Headers.Add("X-Tenant-Id", tenantId);

            Console.WriteLine($"[AuthHandler] Sending {request.Method} {request.RequestUri}");
            if (request.Headers.Authorization != null)
                Console.WriteLine($"[AuthHandler] Header Authorization: {request.Headers.Authorization.Scheme} {request.Headers.Authorization.Parameter}");
            if (request.Headers.Contains("X-Tenant-Id"))
                Console.WriteLine($"[AuthHandler] Header X-Tenant-Id: {string.Join(",", request.Headers.GetValues("X-Tenant-Id"))}");

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
