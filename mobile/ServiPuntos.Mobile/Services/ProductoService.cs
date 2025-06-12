using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public class ProductoService : IProductoService
    {
        private readonly HttpClient _httpClient;

        public ProductoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ProductoCanjeableDto>> GetProductosPorUbicacionAsync(string ubicacionId)
        {

            var list = await _httpClient.GetFromJsonAsync<List<ProductoCanjeableDto>>(
                $"ubicacion/{ubicacionId}/productos");


            return list ?? new List<ProductoCanjeableDto>();
        }

        public async Task<int> GetStockAsync(string ubicacionId, string productoId)
        {

            var resp = await _httpClient.GetAsync(
                $"ubicacion/{ubicacionId}/stock/{productoId}");
            resp.EnsureSuccessStatusCode();


            var stock = await resp.Content.ReadFromJsonAsync<int?>();
            return stock.GetValueOrDefault();
        }
    }
}
