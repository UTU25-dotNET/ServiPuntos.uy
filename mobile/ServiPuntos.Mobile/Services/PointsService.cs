using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using ServiPuntos.Mobile.Models;
using static ServiPuntos.Mobile.Services.AppLogger;

namespace ServiPuntos.Mobile.Services
{
    public class PointsService : IPointsService
    {
        private readonly HttpClient _httpClient;
        public PointsService(HttpClient httpClient) =>
            _httpClient = httpClient;

        public async Task<PointsDto> GetCurrentPointsAsync()
        {
            var userIdString = await SecureStorage.GetAsync("userId");
            if (string.IsNullOrEmpty(userIdString))
                throw new InvalidOperationException("No se encontró el userId en SecureStorage");

            var mensaje = new MensajeNAFTA();
            mensaje.Datos["identificadorUsuario"] = Guid.Parse(userIdString);

            var json = JsonSerializer.Serialize(mensaje);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            LogInfo($"[PointsService] POST /api/nafta/consultar-saldo body: {json}");
            var response = await _httpClient.PostAsync("/api/nafta/consultar-saldo", content);
            response.EnsureSuccessStatusCode();

            var rawText = await response.Content.ReadAsStringAsync();
            LogInfo($"[PointsService] Raw response: {rawText}");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var naftaResp = JsonSerializer.Deserialize<RespuestaNAFTA>(rawText, options)
                            ?? throw new InvalidOperationException("No se pudo parsear la respuesta NAFTA");

            if (naftaResp.Codigo != "OK" ||
                !naftaResp.Datos.TryGetValue("saldoPuntos", out var raw))
            {
                throw new InvalidOperationException(
                    $"Respuesta inesperada: {naftaResp.Codigo} – {naftaResp.Mensaje}");
            }

            int saldo = raw switch
            {
                JsonElement e when e.ValueKind == JsonValueKind.Number => e.GetInt32(),
                JsonElement e when e.ValueKind == JsonValueKind.String => int.Parse(e.GetString()!),
                _ => throw new InvalidOperationException("Formato de saldoPuntos no reconocido")
            };

            return new PointsDto { Points = saldo };
        }
    }
}
