namespace ServiPuntos.Core.Enums
{
    public class UbicacionDto
    {
        public Guid Id { get; set; }
        public string? Nombre { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
        public string? Departamento { get; set; }
        public string? Telefono { get; set; }
        public TimeSpan HoraApertura { get; set; }
        public TimeSpan HoraCierre { get; set; }
        public bool Lavado { get; set; }
        public bool CambioAceite { get; set; }
        public decimal PrecioNaftaSuper { get; set; }
        public decimal PrecioNaftaPremium { get; set; }
        public decimal PrecioDiesel { get; set; }
    }
}
