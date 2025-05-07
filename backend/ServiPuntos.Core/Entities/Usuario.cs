using System;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Email { get; set; }
    public string Telefono { get; set; }
    
    public int Puntos { get; set; }
    //public bool Verificado { get; set; }

    public DateTime FechaNacimiento { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaModificacion { get; set; }
    
    public Guid TenantId { get; set; }
}

