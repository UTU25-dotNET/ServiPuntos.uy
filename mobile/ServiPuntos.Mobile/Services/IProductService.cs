using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetProductsByTenantAsync(Guid tenantId);
        Task<List<ProductDto>> GetProductsByLocationAsync(Guid locationId);
    }
}
