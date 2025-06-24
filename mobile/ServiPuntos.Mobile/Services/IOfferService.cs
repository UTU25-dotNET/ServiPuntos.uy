using System.Collections.Generic;
using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public interface IOfferService
    {
        Task<List<OfferDto>> GetOffersAsync();
    }
}
