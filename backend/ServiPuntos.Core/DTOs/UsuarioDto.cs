namespace ServiPuntos.Core.DTOs
{
    public class UsuarioDto
    {
        public Guid Id { get; set; }              // o int, según tu modelo
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public int? Puntos { get; set; }

        public string? Apellido { get; set; }
        public int? Telefono { get; set; }
        public string? CiudadResidencia { get; set; }
        public string? CombustiblePreferido { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? Intereses { get; set; }
        public string? Password { get; set; } // Solo para cambio de contraseña
    }
}
