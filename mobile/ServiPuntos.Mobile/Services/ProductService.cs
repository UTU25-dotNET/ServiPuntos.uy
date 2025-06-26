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

        public ProductService(HttpClient httpClient) =>
            _httpClient = httpClient;

        public async Task<List<ProductDto>> GetProductsByTenantAsync(Guid tenantId)
        {
#if ANDROID
            Log.Debug(TAG, $"[GetProductsByTenant] tenantId={tenantId}");
#endif
            var url = $"/api/producto/tenant/{tenantId}";
            Console.WriteLine($"[ProductService] GET {_httpClient.BaseAddress}{url}");
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[ProductService] StatusCode={(int)response.StatusCode}, Content={content}");
            response.EnsureSuccessStatusCode();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<List<ProductDto>>(content, options)
                   ?? new List<ProductDto>();
        }

        public async Task<List<ProductDto>> GetProductsByLocationAsync(Guid locationId)
        {
#if ANDROID
            Log.Debug(TAG, $"[GetProductsByLocation] locationId={locationId}");
#endif
            var url = $"/api/productoUbicacion/ubicacion/{locationId}";
            Console.WriteLine($"[ProductService] GET {_httpClient.BaseAddress}{url}");
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[ProductService] StatusCode={(int)response.StatusCode}, Content={content}");
            response.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;
            JsonElement arrayNode = root.ValueKind switch
            {
                JsonValueKind.Object when root.TryGetProperty("datos", out var datosElem) && datosElem.ValueKind == JsonValueKind.Array => datosElem,
                JsonValueKind.Array => root,
                _ => default
            };
            if (arrayNode.ValueKind != JsonValueKind.Array)
                return new List<ProductDto>();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var rawList = JsonSerializer.Deserialize<List<ProductoUbicacionRaw>>(arrayNode.GetRawText(), options)
                          ?? new List<ProductoUbicacionRaw>();

            var result = rawList
                .Where(x => x.ProductoCanjeable != null)
                .Select(x => new ProductDto
                {
                    Id = x.Id,
                    ProductoCanjeableId = x.ProductoCanjeable.Id,
                    Nombre = x.ProductoCanjeable.Nombre,
                    Precio = x.Precio,
                    Categoria = x.Categoria ?? string.Empty,
                    StockDisponible = x.StockDisponible,
                    Activo = x.Activo
                })
                .ToList();
#if ANDROID
            Log.Debug(TAG, $"[GetProductsByLocation] Mapped result.Count={result.Count}");
#endif
            return result;
        }

        public async Task<List<CatalogItemDto>> GetCatalogItemsAsync(Guid locationId, string? categoria = null)
        {
            var url = categoria != null
                ? $"/api/productoUbicacion/ubicacion/{locationId}?categoria={Uri.EscapeDataString(categoria)}"
                : $"/api/productoUbicacion/ubicacion/{locationId}";
            Console.WriteLine($"[ProductService] GET {_httpClient.BaseAddress}{url}");
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[ProductService] StatusCode={(int)response.StatusCode}, Content={content}");
            response.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;
            if (root.ValueKind != JsonValueKind.Array)
                return new List<CatalogItemDto>();

            var lista = new List<CatalogItemDto>();
            foreach (var elem in root.EnumerateArray())
            {
                var canj = elem.GetProperty("productoCanjeable");
                lista.Add(new CatalogItemDto
                {
                    Id = elem.GetProperty("id").GetGuid(),
                    Nombre = canj.GetProperty("nombre").GetString() ?? string.Empty,
                    Descripcion = canj.GetProperty("descripcion").GetString() ?? string.Empty,
                    CostoEnPuntos = canj.GetProperty("costoEnPuntos").GetInt32(),
                    FotoUrl = canj.GetProperty("fotoUrl").GetString() ?? string.Empty
                });
            }
            Console.WriteLine($"[ProductService] Parsed CatalogItems count={lista.Count}");
            return lista;
        }

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
