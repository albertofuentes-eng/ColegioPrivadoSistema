using System.ComponentModel.DataAnnotations;

namespace ColegioPrivado.Models
{
    public class Empresa
    {
        public int EmpresaId { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        public string? Ubicacion { get; set; }

        public bool Activo { get; set; } = true;
    }
}