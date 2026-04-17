using System.ComponentModel.DataAnnotations.Schema;

[Table("DetalleCompra")]
public class DetalleCompra
{
    public int DetalleCompraId { get; set; }

    public int CompraId { get; set; }
    public int ProductoId { get; set; }

    public int Cantidad { get; set; }
    public decimal PrecioCompra { get; set; }

    public Compra? Compra { get; set; }
    public Producto? Producto { get; set; }

    public decimal PrecioUnitario { get; set; }
}