using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public interface IProductoService
    {
        Task<List<ProductoCanjeable>> GetProductosCanjeablesAsync();
        Task<List<ProductoCanjeable>> GetProductosPorUbicacionAsync(string ubicacionId);
    }

    public class ProductoService : IProductoService
    {
        private readonly HttpClient _httpClient;
        public ProductoService(HttpClient httpClient) => _httpClient = httpClient;


        public Task<List<ProductoCanjeable>> GetProductosCanjeablesAsync() =>
            _httpClient.GetFromJsonAsync<List<ProductoCanjeable>>("");


        public Task<List<ProductoCanjeable>> GetProductosPorUbicacionAsync(string ubicacionId) =>
            _httpClient.GetFromJsonAsync<List<ProductoCanjeable>>($"ubicacion/{ubicacionId}");
    }
}
