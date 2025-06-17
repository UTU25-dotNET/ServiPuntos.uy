using System;
using System.ComponentModel.DataAnnotations;

namespace ServiPuntos.WebApp.Models
{
    public class CreateNotificacionViewModel
    {
        [Required]
        [StringLength(100)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Mensaje { get; set; } = string.Empty;

        public Guid? AudienciaId { get; set; }
    }
}
