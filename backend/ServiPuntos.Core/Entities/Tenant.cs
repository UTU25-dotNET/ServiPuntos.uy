public class Tenant
{
    public Guid Id { get; set; } //Globally Unique Identifier
    public string Nombre { get; set; }
    //public string Correo { get; set; }
    //public string Telefono { get; set; }

    public DateTime FechaCreacion { get; set; }
    public DateTime FechaModificacion { get; set; }

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}