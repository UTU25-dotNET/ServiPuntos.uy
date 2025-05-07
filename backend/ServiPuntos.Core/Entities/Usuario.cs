using System;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public int Telefono { get; set; }
    
    public int Puntos { get; set; }
    //public bool Verificado { get; set; }

    public DateTime FechaNacimiento { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaModificacion { get; set; }
    
    required public Guid TenantId { get; set; }

    //Constructor
    public Usuario(string nombre, string email, string password, Guid tenant) {
        Nombre = nombre;
        Email = email;
        Password = password;
        Puntos = 0;
        FechaCreacion = DateTime.Now;
        FechaModificacion = DateTime.Now;
        TenantId = tenant;
    }

}

