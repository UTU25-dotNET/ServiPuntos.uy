using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;
#if ANDROID
using Android.Util;
#endif

namespace ServiPuntos.Mobile.Services
{
    public class CanjeService : ICanjeService
    {
#if ANDROID
        const string TAG = "CanjeService";
#endif
        private readonly HttpClient _httpClient;
        public CanjeService(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<string> GenerarCanjeAsync(Guid usuarioId, Guid productoId, Guid ubicacionId, Guid tenantId)
        {
            const string url = "/api/nafta/generar-canje";

            var mensaje = new
            {
                tipoMensaje = 2,
                ubicacionId,
                tenantId,
                datos = new
                {
                    productoCanjeableId = productoId,
                    usuarioId
                }
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            var payload = JsonSerializer.Serialize(mensaje, options);

#if ANDROID
            Log.Debug(TAG, $"[Request] POST {url}");
            Log.Debug(TAG, $"[Payload] {payload}");
#endif

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            };

#if ANDROID
            foreach (var header in request.Headers)
                Log.Debug(TAG, $"[Request Header] {header.Key}: {string.Join(", ", header.Value)}");
            Log.Debug(TAG, $"[Request Content] {await request.Content.ReadAsStringAsync()}");
#endif

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
#if ANDROID
                Log.Error(TAG, ex.ToString());
#endif
                throw;
            }

            var body = await response.Content.ReadAsStringAsync();

#if ANDROID
            Log.Debug(TAG, $"[Response Status] {(int)response.StatusCode} {response.StatusCode}");
            Log.Debug(TAG, $"[Response Body] {body}");
#endif

            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    using var errDoc = JsonDocument.Parse(body);
                    if (errDoc.RootElement.TryGetProperty("mensaje", out var msgElem))
                        throw new InvalidOperationException(msgElem.GetString());
                }
                catch { }
                throw new InvalidOperationException($"API error {(int)response.StatusCode}: {body}");
            }

            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            if (root.TryGetProperty("codigo", out var codigoElem) &&
                codigoElem.GetString() != "OK")
            {
                var mensajeError = root.GetProperty("mensaje").GetString() ?? "Error desconocido";
#if ANDROID
                Log.Warn(TAG, mensajeError);
#endif
                throw new InvalidOperationException(mensajeError);
            }

            if (root.TryGetProperty("datos", out var datosElem) &&
                datosElem.TryGetProperty("codigoQR", out var qrElem) &&
                qrElem.ValueKind == JsonValueKind.String)
            {
                var codigoQR = qrElem.GetString()!;
#if ANDROID
                Log.Debug(TAG, $"[QR Code] {codigoQR}");
#endif
                return codigoQR;
            }

#if ANDROID
            Log.Warn(TAG, "El servidor no devolvió el código QR en 'datos.codigoQR'");
#endif
            throw new InvalidOperationException("El servidor no devolvió el código QR en 'datos.codigoQR'");
        }
    }
}
