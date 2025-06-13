using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public interface IProductoService
    {
        Task<List<ProductoUbicacionDto>> GetProductosPorUbicacionAsync(string ubicacionId);
        Task<int> GetStockAsync(string ubicacionId, string productoId);
    }

    public class ProductoService : IProductoService
    {
        private readonly HttpClient _httpClient;

        public ProductoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<List<ProductoUbicacionDto>> GetProductosPorUbicacionAsync(string ubicacionId)
        {
            var list = await _httpClient.GetFromJsonAsync<List<ProductoUbicacionDto>>($"ubicacion/{ubicacionId}");
            return list ?? new List<ProductoUbicacionDto>();
        }


        public async Task<int> GetStockAsync(string ubicacionId, string productoId)
        {

            var resp = await _httpClient.GetAsync($"ubicacion/{ubicacionId}/stock");
            resp.EnsureSuccessStatusCode();
            var stockData = await resp.Content.ReadFromJsonAsync<UbicacionStockResponse>();

            var producto = stockData?.ProductosStock?.FirstOrDefault(p => p.ProductoId.ToString() == productoId);
            return producto?.StockDisponible ?? 0;
        }
    }


    public class ProductoUbicacionDto
    {
        public string Id { get; set; }
        public string UbicacionId { get; set; }
        public string ProductoCanjeableId { get; set; }
        public string ProductoNombre { get; set; }
        public int StockDisponible { get; set; }
        public bool Activo { get; set; }
    }

    public class UbicacionStockResponse
    {
        public string UbicacionId { get; set; }
        public string UbicacionNombre { get; set; }
        public List<ProductoStockDto> ProductosStock { get; set; }
    }

    public class ProductoStockDto
    {
        public string Id { get; set; }
        public string ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public int StockDisponible { get; set; }
        public bool Activo { get; set; }
    }
}
