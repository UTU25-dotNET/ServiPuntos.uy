using System.Threading.Tasks;
using ServiPuntos.Core.NAFTA;

namespace ServiPuntos.Core.Interfaces
{
    public interface IPointsRuleEngine
    {
        Task<decimal> CalcularPuntosAsync(TransaccionNAFTA transaccion, int tenantId);
    }
}