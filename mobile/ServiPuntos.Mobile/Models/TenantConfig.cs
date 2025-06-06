namespace ServiPuntos.Mobile.Models
{
    public class TenantConfig
    {
        public string Id { get; set; } // **DEBE SER STRING**
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }
    }
}