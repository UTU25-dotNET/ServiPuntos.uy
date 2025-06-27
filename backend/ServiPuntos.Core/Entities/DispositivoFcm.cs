namespace ServiPuntos.Core.Entities
{
    public class DispositivoFcm
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }

        public Usuario? Usuario { get; set; }
    }
}
