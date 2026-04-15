using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Proveedor")]
public class Proveedor
{
    public int ProveedorId { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio")]
    [Phone(ErrorMessage = "Teléfono no válido")]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección es obligatoria")]
    [StringLength(200, ErrorMessage = "Máximo 200 caracteres")]
    public string Direccion { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    public decimal CreditoDisponible { get; set; } = 0;

    public int EmpresaId { get; set; }
}