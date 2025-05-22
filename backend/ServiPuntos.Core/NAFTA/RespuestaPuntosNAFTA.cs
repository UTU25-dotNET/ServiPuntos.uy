namespace ServiPuntos.Core.NAFTA
{
    public class RespuestaPuntosNAFTA
    {
        public Guid IdentificadorUsuario { get; set; }
        public int PuntosOtorgados { get; set; }
        public int SaldoActual { get; set; }
        public int SaldoAnterior { get; set; }
    }
}