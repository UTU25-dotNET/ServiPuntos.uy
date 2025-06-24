using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public interface IPointsService
    {
        Task<PointsDto> GetCurrentPointsAsync();
    }
}
