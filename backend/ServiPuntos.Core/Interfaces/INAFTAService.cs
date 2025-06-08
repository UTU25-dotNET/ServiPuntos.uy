using System.Threading.Tasks;
using ServiPuntos.Core.NAFTA;

namespace ServiPuntos.Core.Interfaces
{
    public interface INAFTAService
    {
        Task<RespuestaNAFTA> ProcesarTransaccionAsync(MensajeNAFTA mensaje);
        Task<RespuestaNAFTA> VerificarUsuarioAsync(MensajeNAFTA mensaje);
        Task<RespuestaNAFTA> ProcesarCanjeAsync(MensajeNAFTA mensaje);
        Task<RespuestaNAFTA> ConsultarSaldoAsync(MensajeNAFTA mensaje);
        Task<RespuestaNAFTA> ConfirmarPagoPayPalAsync(MensajeNAFTA mensaje);
        Task<RespuestaNAFTA> GenerarCanjeAsync(MensajeNAFTA mensaje);
        Task<RespuestaNAFTA> GenerarCanjesAsync(MensajeNAFTA mensaje);
    }
}