namespace ServiPuntos.Mobile.Models
{
    public class GenerarCanjeDataDto
    {
        public string CodigoQR { get; set; }
    }

    public class GenerarCanjeResponseDto
    {
        public string Codigo { get; set; }
        public string Mensaje { get; set; }
        public GenerarCanjeDataDto Datos { get; set; }
    }
}
