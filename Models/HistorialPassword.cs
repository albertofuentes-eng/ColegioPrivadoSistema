using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColegioPrivado.Models
{
    public class HistorialPassword
    {
        [Key]
        public int IdHistorial { get; set; }

        public int IdUsuario { get; set; }

        public string PasswordHash { get; set; } = "";

        public string Salt { get; set; } = "";

        public string PasswordSoundex { get; set; } = "";

        public DateTime FechaCambio { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuario? Usuario { get; set; }
    }
}
