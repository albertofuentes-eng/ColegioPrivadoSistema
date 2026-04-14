using System;
using System.Collections.Generic;

public class DevolucionCompra
{
    public int DevolucionCompraId { get; set; }

    public int CompraId { get; set; }

    public DateTime Fecha { get; set; }

    public string Motivo { get; set; } = "";

    public List<DetalleDevolucion> Detalles { get; set; } = new();

    public bool EsCredito { get; set; } = false;
}