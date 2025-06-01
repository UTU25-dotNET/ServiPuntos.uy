namespace ServiPuntos.Core.Entities
{
    public class Tenant
    {
        public Guid Id { get; set; } //Globally Unique Identifier
        required public string Nombre { get; set; }

        public string? LogoUrl { get; set; }
        public string? Color { get; set; } //no tengo idea como poner un color aca supongo que el formato es un string y sera algo como #FFFFFF
        public string NombrePuntos { get; set; } = "puntos"; //nombre del punto, por ejemplo "Puntos ServiPuntos"

        // Configuracion Global
        public decimal ValorPunto { get; set; }
        public decimal TasaPuntos { get; set; }
        public int DiasCaducidadPuntos { get; set; } = 365; //dias de caducidad de los puntos, por defecto 365 dias

        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }

        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
        public ICollection<Ubicacion> Ubicaciones { get; set; } = new List<Ubicacion>();
        public ICollection<Audiencia> Audiencias { get; set; } = new List<Audiencia>();

        //Constructor
        public Tenant() {
            Id = Guid.NewGuid();
            FechaCreacion = DateTime.UtcNow;
        }
    }

}