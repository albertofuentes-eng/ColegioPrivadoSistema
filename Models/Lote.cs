using System;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Lote")]
public class Lote
{
    public int LoteId { get; set; }

    public int ProductoId { get; set; }
    public int ProveedorId { get; set; }

    public string NumeroLote { get; set; } = "";
    public decimal PrecioCompra { get; set; }
    public int CantidadComprada { get; set; }

    public DateTime FechaCompra { get; set; }

    public int EmpresaId { get; set; }

    // Navegación
    public Producto? Producto { get; set; }
    public Proveedor? Proveedor { get; set; }
}