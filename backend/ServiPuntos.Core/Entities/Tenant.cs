namespace ServiPuntos.Core.Entities
{
    public class Tenant
    {
        public Guid Id { get; set; } //Globally Unique Identifier
        required public string Nombre { get; set; }
        //public string Correo { get; set; }
        //public string Telefono { get; set; }
        public string? LogoUrl { get; set; }
        public string? Color { get; set; } //no tengo idea como poner un color aca supongo que el formato es un string y sera algo como #FFFFFF
        public decimal ValorPunto { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }

        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

        //Constructor
        public Tenant() { }
    }

}