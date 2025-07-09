using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public interface ICatalogService
    {
        Task<IEnumerable<ProductoUbicacionDto>> GetProductsByLocationAsync(Guid ubicacionId, string? categoria);

        Task<IEnumerable<ProductoUbicacionDto>> GetAllProductsAsync();
    }
}
