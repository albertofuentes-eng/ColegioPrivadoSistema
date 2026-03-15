using System.ComponentModel.DataAnnotations;

namespace ColegioPrivado.Models
{
    public class Estudiante
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        public string Apellido { get; set; } = string.Empty;

        public int Edad { get; set; }

        public string Grado { get; set; } = string.Empty;
    }
}