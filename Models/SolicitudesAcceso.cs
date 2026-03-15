using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColegioPrivado.Models
{
    [Table("SolicitudesAcceso")]
    public class SolicitudesAcceso
    {
        [Key]
        public int IdSolicitud { get; set; }

        public string? Email { get; set; }

        public string? NivelSolicitado { get; set; }

        public string? CodigoAcceso { get; set; }

        public string? Estado { get; set; }

        public DateTime FechaSolicitud { get; set; }
    }
}