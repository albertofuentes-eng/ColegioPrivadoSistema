using System;
using System.ComponentModel.DataAnnotations.Schema;

[Table("ProductoPendiente")]
public class ProductoPendiente
{
    public int ProductoPendienteId { get; set; }

    public int ProductoId { get; set; }
    public int CompraId { get; set; }

    public int Cantidad { get; set; }
    public int CantidadProcesada { get; set; } = 0;

    public DateTime FechaCompra { get; set; }

    // 📊 Estados: Pendiente, Procesado, Cancelado
    public string Estado { get; set; } = "Pendiente";

    public Producto? Producto { get; set; }
    public Compra? Compra { get; set; }

    public int EmpresaId { get; set; }
}
