namespace ServiPuntos.Mobile.Models
{
    public class RegisterRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Ci { get; set; } = string.Empty;
        public Guid TenantId { get; set; }
    }
}