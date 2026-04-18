using ColegioPrivado.Data;
using ColegioPrivado.Models;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Servicio para validar reglas de devolución de compras
/// Centraliza la lógica de validación sin modificar el controlador
/// </summary>
public class ValidadorDevoluciones
{
    private readonly ApplicationDbContext _context;

    public ValidadorDevoluciones(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Valida si una devolución puede procesarse según las reglas de negocio
    /// </summary>
    public (bool esValido, List<string> errores) Validar(
        int compraId, 
        List<DetalleDevolucion> detalles, 
        bool esCredito)
    {
        var errores = new List<string>();

        // Validar que exista la compra
        var compra = _context.Compras.Find(compraId);
        if (compra == null)
        {
            errores.Add("❌ La compra relacionada no existe.");
            return (false, errores);
        }

        // Validar límite de tiempo de 7 días
        var diasDesdeCompra = (DateTime.Now.Date - compra.Fecha.Date).TotalDays;
        if (diasDesdeCompra > 7)
        {
            errores.Add("⏰ No se permite devolución después de 7 días desde la compra.");
        }

        // Obtener los productos y detalles de la compra original
        var detalleCompraOriginal = _context.DetalleCompras
            .Where(dc => dc.CompraId == compraId)
            .ToList();

        // Validar productos NO ofertados y dañados
        foreach (var detalle in detalles)
        {
            var detalleOriginal = detalleCompraOriginal
                .FirstOrDefault(dc => dc.ProductoId == detalle.ProductoId);

            if (detalleOriginal == null)
            {
                errores.Add($"❌ El producto {detalle.ProductoId} no existe en esta compra.");
                continue;
            }

            // Validación 1: Si fue comprado en oferta, NO puede devolverse
            if (detalleOriginal.EsOfertado)
            {
                errores.Add($"❌ El producto {detalle.ProductoId} fue comprado en oferta y no puede devolverse.");
            }

            // Validación 2: Si no es crédito, DEBE estar marcado como dañado
            if (!esCredito && !detalle.Danado)
            {
                errores.Add($"❌ El producto {detalle.ProductoId} debe estar marcado como dañado para su devolución (a menos que sea por crédito).");
            }

            // Validación 3: También verificar EnOferta del producto
            var producto = _context.Productos.Find(detalle.ProductoId);
            if (producto?.EnOferta ?? false)
            {
                errores.Add($"⚠️ El producto {detalle.ProductoId} está en oferta en catálogo y no puede devolverse.");
            }
        }

        bool esValido = errores.Count == 0;
        return (esValido, errores);
    }

    /// <summary>
    /// Obtiene información de la oferta de un producto en una compra
    /// </summary>
    public (bool esOfertado, string numeroLote) ObtenerInfoOferta(int compraId, int productoId)
    {
        var detalle = _context.DetalleCompras
            .FirstOrDefault(dc => dc.CompraId == compraId && dc.ProductoId == productoId);

        if (detalle == null)
            return (false, "");

        return (detalle.EsOfertado, detalle.NumeroLote);
    }

    /// <summary>
    /// Obtiene todos los detalles de compra que NO fueron ofertados
    /// </summary>
    public List<DetalleCompra> ObtenerDetallesNoOfertados(int compraId)
    {
        return _context.DetalleCompras
            .Where(dc => dc.CompraId == compraId && !dc.EsOfertado)
            .ToList();
    }

    /// <summary>
    /// Obtiene todos los detalles de compra que SÍ fueron ofertados
    /// </summary>
    public List<DetalleCompra> ObtenerDetallesOfertados(int compraId)
    {
        return _context.DetalleCompras
            .Where(dc => dc.CompraId == compraId && dc.EsOfertado)
            .ToList();
    }
}
