using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _httpClient;

        public CatalogService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ProductoUbicacionDto>> GetProductsByLocationAsync(Guid ubicacionId, string? categoria)
        {
            var url = $"api/ProductoUbicacion/ubicacion/{ubicacionId}";
            if (!string.IsNullOrEmpty(categoria))
                url += $"?categoria={Uri.EscapeDataString(categoria)}";

            return await _httpClient.GetFromJsonAsync<IEnumerable<ProductoUbicacionDto>>(url)
                   ?? Array.Empty<ProductoUbicacionDto>();
        }

        public async Task<IEnumerable<ProductoUbicacionDto>> GetAllProductsAsync()
        {
            // Llama al GET api/ProductoUbicacion
            return await _httpClient.GetFromJsonAsync<IEnumerable<ProductoUbicacionDto>>("api/ProductoUbicacion")
                   ?? Array.Empty<ProductoUbicacionDto>();
        }
    }
}
