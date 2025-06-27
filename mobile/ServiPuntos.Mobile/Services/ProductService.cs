using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;
#if ANDROID
using Android.Util;
#endif

namespace ServiPuntos.Mobile.Services
{
    public class ProductService : IProductService
    {
#if ANDROID
        const string TAG = "ProductService";
#endif
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
            => _httpClient = httpClient;

        public async Task<List<ProductDto>> GetProductsByTenantAsync(Guid tenantId)
        {
#if ANDROID
            Log.Debug(TAG, $"[GetProductsByTenant] tenantId={tenantId}");
#endif
            var url = $"/api/producto/tenant/{tenantId}";
            var rsp = await _httpClient.GetAsync(url);
#if ANDROID
            Log.Debug(TAG, $"[GetProductsByTenant] HTTP {url} ⇒ {(int)rsp.StatusCode}");
#endif
            rsp.EnsureSuccessStatusCode();

            var json = await rsp.Content.ReadAsStringAsync();
#if ANDROID
            Log.Debug(TAG, $"[GetProductsByTenant] Raw JSON: {json}");
#endif
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var list = JsonSerializer
                .Deserialize<List<ProductDto>>(json, options)
                ?? new List<ProductDto>();

#if ANDROID
            Log.Debug(TAG, $"[GetProductsByTenant] Deserializados list.Count={list.Count}");
#endif
            return list;
        }

        public async Task<List<ProductDto>> GetProductsByLocationAsync(Guid locationId)
        {
#if ANDROID
            Log.Debug(TAG, $"[GetProductsByLocation] locationId={locationId}");
#endif
            var url = $"/api/productoUbicacion/ubicacion/{locationId}";
            var rsp = await _httpClient.GetAsync(url);
#if ANDROID
            Log.Debug(TAG, $"[GetProductsByLocation] HTTP {url} ⇒ {(int)rsp.StatusCode}");
#endif
            rsp.EnsureSuccessStatusCode();

            var json = await rsp.Content.ReadAsStringAsync();
#if ANDROID
            Log.Debug(TAG, $"[GetProductsByLocation] Raw JSON: {json}");
#endif

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            JsonElement arrayNode;

            if (root.ValueKind == JsonValueKind.Object
                && root.TryGetProperty("datos", out var datosElem)
                && datosElem.ValueKind == JsonValueKind.Array)
            {
                arrayNode = datosElem;
#if ANDROID
                Log.Debug(TAG, "[GetProductsByLocation] Usando propiedad 'datos' como array");
#endif
            }
            else if (root.ValueKind == JsonValueKind.Array)
            {
                arrayNode = root;
#if ANDROID
                Log.Debug(TAG, "[GetProductsByLocation] JSON raíz es ya un array");
#endif
            }
            else
            {
#if ANDROID
                Log.Warn(TAG, "[GetProductsByLocation] Formato de JSON inesperado");
#endif
                return new List<ProductDto>();
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var rawList = JsonSerializer
                .Deserialize<List<ProductoUbicacionRaw>>(arrayNode.GetRawText(), options)
                ?? new List<ProductoUbicacionRaw>();

#if ANDROID
            Log.Debug(TAG, $"[GetProductsByLocation] Deserializados rawList.Count={rawList.Count}");
#endif

            var result = rawList
                .Where(x => x.ProductoCanjeable != null)
                .Select(x => new ProductDto
                {
                    // Id aquí opcional: puede usarse para tracking interno
                    Id = x.Id,
                    ProductoCanjeableId = x.ProductoCanjeable.Id,
                    Nombre = x.ProductoCanjeable.Nombre,
                    Precio = x.Precio
                })
                .ToList();

#if ANDROID
            Log.Debug(TAG, $"[GetProductsByLocation] Mapeados result.Count={result.Count}");
#endif

            return result;
        }

        // --- Modelos internos para deserializar productoUbicacion ---

        private class ProductoUbicacionRaw
        {
            public Guid Id { get; set; }
            public string? Categoria { get; set; }
            public decimal Precio { get; set; }
            public int StockDisponible { get; set; }
            public bool Activo { get; set; }
            public ProductoInfo ProductoCanjeable { get; set; } = null!;
        }

        private class ProductoInfo
        {
            public Guid Id { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string? Descripcion { get; set; }
            public int CostoEnPuntos { get; set; }
            public string? FotoUrl { get; set; }
        }
    }
}
