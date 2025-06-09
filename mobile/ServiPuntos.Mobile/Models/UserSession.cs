namespace ServiPuntos.Mobile.Models
{
    public class UserSession
    {
        public string Token { get; set; }
        public string Nombre { get; set; }
        public string Rol { get; set; }
        public bool IsAuthenticated => !string.IsNullOrEmpty(Token);
    }
}
