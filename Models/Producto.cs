using System.ComponentModel.DataAnnotations.Schema;

[Table("Producto")]
public class Producto
{
    public int ProductoId { get; set; }

    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";

    public int Stock { get; set; }

    public decimal PrecioCompra { get; set; }
    public decimal PrecioVenta { get; set; }

    public bool Activo { get; set; }
    public bool EnOferta { get; set; } = false;
}