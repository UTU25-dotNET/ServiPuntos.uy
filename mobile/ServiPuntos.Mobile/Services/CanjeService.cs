// CanjeService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    // Refit API interface
    public interface ICanjeApi
    {
        [Post("/generar-canje")]
        Task<CanjeResponse> GenerarCanjeAsync([Body] CanjeRequest request);

        [Get("/usuario/{userId}")]
        Task<IEnumerable<CanjeHistorialItem>> GetHistorialAsync(string userId);

        [Post("/{canjeId}/confirmar")]
        Task<ApiResponse<object>> ConfirmarCanjeAsync(Guid canjeId);
    }

    // Service interface + implementation
    public interface ICanjeService
    {
        Task<CanjeResponse> GenerarCanjeAsync(CanjeRequest body);
        Task<IEnumerable<CanjeHistorialItem>> GetHistorialAsync(string userId);
        Task<bool> ValidateQrAsync(string codigoQr);
    }

    public class CanjeService : ICanjeService
    {
        private readonly ICanjeApi _api;

        public CanjeService(ICanjeApi api)
        {
            _api = api;
        }

        public async Task<CanjeResponse> GenerarCanjeAsync(CanjeRequest body)
        {
            return await _api.GenerarCanjeAsync(body);
        }

        public async Task<IEnumerable<CanjeHistorialItem>> GetHistorialAsync(string userId)
        {
            return await _api.GetHistorialAsync(userId) ?? Array.Empty<CanjeHistorialItem>();
        }

        public async Task<bool> ValidateQrAsync(string codigoQr)
        {
            if (!Guid.TryParse(codigoQr, out var canjeId))
                throw new ArgumentException("Código QR inválido: no es un GUID.", nameof(codigoQr));

            var response = await _api.ConfirmarCanjeAsync(canjeId);
            return response.IsSuccessStatusCode;
        }
    }
}
