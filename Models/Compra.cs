using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Compra")]
public class Compra
{

    public int CompraId { get; set; }
    public DateTime Fecha { get; set; }
    public int ProveedorId { get; set; }
    public decimal Total { get; set; }

    public Proveedor? Proveedor { get; set; }
    public List<DetalleCompra>? Detalles { get; set; }
}