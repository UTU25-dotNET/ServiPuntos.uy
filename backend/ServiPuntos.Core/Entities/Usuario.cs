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
        public string? CiudadResidencia { get; set; } //ciudad del usuario, puede ser nulo si no se especifica

        // Audiencia Segmentacion 
        public int Puntos { get; set; }

        public string? UltimaCategoriaComprada { get; set; } //ultima categoria comprada por el usuario, puede ser nulo si no ha comprado nada
        
        public bool EsSubscriptorPremium { get; set; } = false; //indica si el usuario es subscriptor premium, por defecto es false
        public bool VerificadoVEAI { get; set; }
        public bool Bloqueado { get; set; } = false; //si esta bloqueado no puede hacer login, se desbloquea automaticamente al pasar un tiempo determinado
        public int IntentosFallidos { get; set; } = 0; //para el login, si llega a 3 se bloquea la cuenta por un tiempo

        public int TotalVisitas { get; set; } = 0; //total de visitas a ubicaciones
        public int TotalCompras { get; set; } = 0; //total de compras realizadas
        public decimal GastoPromedio { get; set; } = 0; //gasto promedio en las ubicaciones del tenant
        public decimal GastoTotal { get; set; } = 0; //gasto total en el tenant, se actualiza automaticamente al visitar una ubicacion y hacer una compra
        
        public decimal VisitasPorMes { get; set; } = 0; //visitas por mes, se reinicia cada mes
        public int PuntosUtilizados { get; set; } = 0; //puntos gastados
        public string CombustiblePreferido { get; set; } = string.Empty; //tipo de combustible preferido

        public SegmentoCliente SegmentoClientes { get; set; } = SegmentoCliente.Nuevo; //por defecto es nuevo, se actualiza automaticamente segun los criterios del tenant

        public Guid UbicacionPreferida { get; set; } = Guid.Empty; //ubicacion preferida del usuario, se actualiza automaticamente al visitar una ubicacion

        public List<string> Intereses { get; set; } = new List<string>(); // Ejemplo de propiedad de lista

        public Guid? SegmentoDinamicoId { get; set; }// ID del segmento dinámico al que pertenece

        public DateTime? UltimaVisita { get; set; } //ultima visita a una ubicacion
        public DateTime? FechaNacimiento { get; set; }
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
            FechaCreacion = DateTime.UtcNow;
            FechaModificacion = DateTime.UtcNow;
            TenantId = tenantId;
            Rol = rol;
        }

        public bool VerificarPassword(string passwordPlano)
        {
            return BCrypt.Net.BCrypt.Verify(passwordPlano, Password);
        }

        public void ActualizarMetricasCompra(decimal montoGastado, string tipoCombustible, Guid ubicacionId)
        {
            TotalVisitas++;
            GastoTotal += montoGastado;
            UltimaVisita = DateTime.UtcNow;
            CombustiblePreferido = tipoCombustible;
            UbicacionPreferida = ubicacionId;

            // Calcular visitas por mes basado en tiempo desde creación
            var mesesDesdeCreacion = (DateTime.UtcNow - FechaCreacion).Days / 30.0;
            if (mesesDesdeCreacion > 0)
            {
                VisitasPorMes = (decimal)(TotalVisitas / mesesDesdeCreacion);
            }

            FechaModificacion = DateTime.UtcNow;
        }

        /// <summary>
        /// Método helper para registrar el uso de puntos
        /// </summary>
        public void UtilizarPuntos(int puntosUsados)
        {
            if (Puntos >= puntosUsados)
            {
                Puntos -= puntosUsados;
                PuntosUtilizados += puntosUsados;
                FechaModificacion = DateTime.UtcNow;
            }
        }
    }

}

