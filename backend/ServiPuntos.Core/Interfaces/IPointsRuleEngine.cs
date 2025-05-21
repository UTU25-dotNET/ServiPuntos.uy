using System.Threading.Tasks;
using ServiPuntos.Core.NAFTA;

namespace ServiPuntos.Core.Interfaces
{
    public interface IPointsRuleEngine
    {
        Task<int> CalcularPuntosAsync(TransaccionNAFTA transaccion, Guid tenantId);
    }
}