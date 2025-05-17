using System.Diagnostics.CodeAnalysis;
using ServiPuntos.Core.Enums;
namespace ServiPuntos.Core.Entities
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public required Guid TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required int  Ci { get; set; }

        public required RolUsuario Rol { get; set; } = RolUsuario.UsuarioFinal; //por defecto es usuario final

        public int Telefono { get; set; }

        public int Puntos { get; set; }
        //public bool Verificado { get; set; }

        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }

        //Constructor
        public Usuario() { }

        [SetsRequiredMembers]
        public Usuario(string nombre, string email, string password, int ci, Guid tenantId, RolUsuario rol)
        {
            Nombre = nombre;
            Email = email;
            Password = BCrypt.Net.BCrypt.HashPassword(password);
            Ci = ci;
            Puntos = 0;
            FechaCreacion = DateTime.Now;
            FechaModificacion = DateTime.Now;
            TenantId = tenantId;
            Rol = rol;
        }

        public bool VerificarPassword(string passwordPlano)
        {
            return BCrypt.Net.BCrypt.Verify(passwordPlano, Password);
        }
    }

}

