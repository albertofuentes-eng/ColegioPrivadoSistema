using System;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Factura")]
public class Factura
{
    public int FacturaId { get; set; }

    public string NumeroFactura { get; set; } = "";

    public DateTime Fecha { get; set; }

    public decimal Total { get; set; }

    public int CompraId { get; set; }

    public int EmpresaId { get; set; }

    public Compra? Compra { get; set; }
}
