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

    // 🆕 Nuevos campos para rastreo de lotes y ofertas
    public string NumeroLote { get; set; } = "";
    
    public bool EsOfertado { get; set; } = false;
}