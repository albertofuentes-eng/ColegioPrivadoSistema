using System.ComponentModel.DataAnnotations;

namespace ColegioPrivado.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required]
        public string UsuarioNombre { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Salt { get; set; } = string.Empty;

        public int IntentosFallidos { get; set; } = 0;

        public bool Estado { get; set; } = true;

        public int IdRol { get; set; }

        public bool EsPrimerInicio { get; set; } = true;

        public DateTime? FechaUltimoAcceso { get; set; }

        public string? Rol { get; set; }
        public string? Email { get; set; }

        public string? CodigoRecuperacion { get; set; }

        public DateTime? FechaExpiracionCodigo { get; set; }
    }
}