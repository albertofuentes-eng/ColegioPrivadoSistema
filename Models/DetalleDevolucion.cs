public class DetalleDevolucion
{
    public int DetalleDevolucionId { get; set; }

    public int DevolucionCompraId { get; set; }

    public int ProductoId { get; set; }

    public int Cantidad { get; set; }

    public bool Danado { get; set; } = false;
}