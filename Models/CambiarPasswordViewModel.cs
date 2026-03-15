using System.ComponentModel.DataAnnotations;

namespace ColegioPrivado.Models
{
    public class CambiarPasswordViewModel
    {
        [Required]
        public string NuevaPassword { get; set; } = string.Empty;

        [Required]
        public string ConfirmarPassword { get; set; } = string.Empty;

        public string? Mensaje { get; set; }
    }
}