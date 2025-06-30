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

        public CanjeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GenerarCanjeAsync(Guid usuarioId, Guid productoId, Guid ubicacionId, Guid tenantId)
        {
            const string url = "/api/nafta/generar-canje";
#if ANDROID
            Log.Info(TAG, $"[GenerarCanje] POST {url}");
#endif
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
            Log.Info(TAG, $"[GenerarCanje] Payload: {payload}");
#endif

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            };

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
            Log.Info(TAG, $"[GenerarCanje] Status: {(int)response.StatusCode}");
            Log.Info(TAG, $"[GenerarCanje] Body: {body}");
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
                Log.Info(TAG, $"[GenerarCanje] QR: {codigoQR}");
#endif
                return codigoQR;
            }

#if ANDROID
            Log.Warn(TAG, "No se encontró 'datos.codigoQR' en la respuesta");
#endif
            throw new InvalidOperationException("El servidor no devolvió el código QR en 'datos.codigoQR'");
        }

        public async Task<CanjeListResponseDto> GetCanjesByUsuarioAsync(Guid usuarioId, Guid? cursor = null, int limit = 20)
        {
            var url = $"/api/canje/usuario/{usuarioId}?limit={limit}";
            if (cursor.HasValue)
                url += $"&cursor={cursor.Value}";

#if ANDROID
            Log.Info(TAG, $"[GetCanjes] GET {url}");
#endif
            HttpResponseMessage response;
            try
            {
                response = await _httpClient.GetAsync(url);
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
            Log.Info(TAG, $"[GetCanjes] Status: {(int)response.StatusCode}");
            Log.Info(TAG, $"[GetCanjes] Body: {body}");
#endif
            response.EnsureSuccessStatusCode();

            var result = JsonSerializer.Deserialize<CanjeListResponseDto>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

#if ANDROID
            Log.Info(TAG, $"[GetCanjes] Items={result.Items.Count}, NextCursor={result.NextCursor}");
#endif
            return result;
        }
    }
}
