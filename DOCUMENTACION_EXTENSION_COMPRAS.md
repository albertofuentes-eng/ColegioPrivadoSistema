# 📦 DOCUMENTACIÓN: EXTENSIÓN DE COMPRAS Y DEVOLUCIONES

## ✅ Cambios Realizados

### 1. **Modelos Extendidos**

#### DetalleCompra (EXTENDIDO)
```csharp
public class DetalleCompra
{
    // Campos existentes...
    
    // 🆕 NUEVOS CAMPOS
    public string NumeroLote { get; set; } = "";        // Identificador único del lote
    public bool EsOfertado { get; set; } = false;       // ¿Fue comprado en oferta?
}
```

**Función:**
- `NumeroLote`: Rastrea cada línea de compra de forma única. Formato: `LOTE-{ProveedorId}-{CompraId}-{Timestamp}`
- `EsOfertado`: Indica si la línea específica fue comprada en oferta (bloquea devoluciones)

#### ProductoPendiente (NUEVO)
```csharp
public class ProductoPendiente
{
    public int ProductoPendienteId { get; set; }
    public int ProductoId { get; set; }
    public int CompraId { get; set; }
    public int Cantidad { get; set; }
    public int CantidadProcesada { get; set; }          // Cantidad ya devuelta/procesada
    public DateTime FechaCompra { get; set; }
    public string Estado { get; set; } = "Pendiente";   // "Pendiente", "Procesado", "Cancelado"
    public int EmpresaId { get; set; }
}
```

**Función:**
- Rastrea todos los productos comprados que aún están pendientes de procesar
- Permite seguimiento de devoluciones y procesamiento
- Estados: `Pendiente` → `Procesado` (devuelto) o `Cancelado` (crédito)

---

### 2. **Controladores**

#### ComprasController (MEJORADO)
**Cambios:**
- ✅ Genera `NumeroLote` automático para cada línea de compra
- ✅ Crea registros en `ProductosPendientes` con estado "Pendiente"
- ✅ Mantiene lógica existente de stock e intacta
- ✅ Permite marcar `EsOfertado` al crear la compra

**Flujo:**
```
1. Admin crea compra con productos
2. Sistema genera NumeroLote único para cada línea
3. Stock se incrementa
4. Se crea registro ProductoPendiente para cada producto
5. Si hay crédito de proveedor, se deduce automáticamente
```

#### DevolucionesController (VALIDACIONES MEJORADAS)
**Nuevas Validaciones:**
- ✅ Verifica que `EsOfertado = false` en línea de compra
- ✅ Verifica que `Danado = true` (a menos que sea crédito)
- ✅ Verifica que no pasen 7 días
- ✅ Valida también `EnOferta` del producto en catálogo
- ✅ Actualiza estado de `ProductosPendientes`

**Flujo de Devolución (Dañado):**
```
1. Se valida que NO fue ofertado (EsOfertado = false)
2. Se valida que esté marcado como Danado = true
3. Stock se REDUCE (antes se aumentaba incorrectamente)
4. ProductoPendiente se marca como "Procesado"
5. Se crea DetalleDevolucion asociado
```

**Flujo de Devolución (Crédito):**
```
1. Se marca EsCredito = true
2. Se incrementa CreditoDisponible del proveedor
3. ProductoPendiente se marca como "Cancelado"
4. Stock NO se modifica
```

#### ProductosPendientesController (NUEVO)
**Funciones:**
- 📋 **Index**: Lista productos pendientes por estado (Pendiente/Procesado/Cancelado)
- 🔍 **Detalles**: Ver información detallada de un producto pendiente
- ✏️ **ActualizarEstado**: Cambiar estado manualmente si es necesario
- 📊 **Resumen**: Dashboard con conteos totales

---

### 3. **Validador Centralizado**

#### ValidadorDevoluciones (NUEVO)
Centraliza toda la lógica de validación de devoluciones.

**Métodos Disponibles:**
```csharp
// Validación principal
(bool esValido, List<string> errores) Validar(
    int compraId, 
    List<DetalleDevolucion> detalles, 
    bool esCredito)

// Obtener información de oferta
(bool esOfertado, string numeroLote) ObtenerInfoOferta(int compraId, int productoId)

// Filtros
List<DetalleCompra> ObtenerDetallesNoOfertados(int compraId)
List<DetalleCompra> ObtenerDetallesOfertados(int compraId)
```

**Ventajas:**
- ✅ Lógica centralizada y reutilizable
- ✅ Fácil de testear
- ✅ Mensajes de error claros
- ✅ Separación de responsabilidades

---

## 📊 Reglas de Negocio Implementadas

### Compras
| Regla | Implementación | Status |
|-------|---------------|--------|
| Guardar precio unitario por producto | DetalleCompra.PrecioUnitario | ✅ Existía |
| Guardar proveedor asociado | Compra.ProveedorId | ✅ Existía |
| Número de lote por línea | DetalleCompra.NumeroLote | ✅ NUEVA |
| Diferentes precios por proveedor | DetalleCompra.PrecioUnitario | ✅ Existía |
| Indicar si fue ofertado | DetalleCompra.EsOfertado | ✅ NUEVA |
| Stock se reduce con devolución | DevolucionesController | ✅ MEJORADA |

### Devoluciones
| Regla | Implementación | Status |
|-------|---------------|--------|
| NO devolver si EsOfertado=true | ValidadorDevoluciones | ✅ NUEVA |
| SOLO devolver si EsDañado=true | ValidadorDevoluciones | ✅ NUEVA |
| Validar 7 días desde compra | ValidadorDevoluciones | ✅ Existía |
| Nota de crédito genera saldo | Proveedor.CreditoDisponible | ✅ Existía |

### Productos Pendientes
| Regla | Implementación | Status |
|-------|---------------|--------|
| Rastrear productos por compra | ProductoPendiente | ✅ NUEVA |
| Estados: Pendiente/Procesado/Cancelado | ProductosPendientesController | ✅ NUEVA |
| Actualizar con devoluciones | DevolucionesController | ✅ NUEVA |
| Dashboard de resumen | ProductosPendientesController.Resumen | ✅ NUEVA |

---

## 🔍 Casos de Uso

### Caso 1: Compra Normal
```
Admin → Crea Compra → Sistema:
  ✓ Genera NumeroLote: "LOTE-1-5-12345678901234"
  ✓ EsOfertado = false (por defecto)
  ✓ Stock += Cantidad
  ✓ Crea ProductoPendiente (Estado: "Pendiente")
```

### Caso 2: Compra en Oferta
```
Admin → Crea Compra → Marca EsOfertado = true → Sistema:
  ✓ Genera NumeroLote: "LOTE-1-5-12345678901234"
  ✓ EsOfertado = true
  ✓ Stock += Cantidad
  ✓ Crea ProductoPendiente
  
Intento de Devolución → BLOQUEADO ❌
"No se permite devolución de productos comprados en oferta"
```

### Caso 3: Devolución de Producto Dañado
```
Admin → Crea Devolución → Marca Danado = true → Sistema:
  ✓ Valida: EsOfertado = false ✓
  ✓ Valida: Danado = true ✓
  ✓ Stock -= Cantidad
  ✓ ProductoPendiente.Estado = "Procesado"
  ✓ Crea DetalleDevolucion
```

### Caso 4: Devolución por Crédito
```
Admin → Crea Devolución → Marca EsCredito = true → Sistema:
  ✓ Valida: EsOfertado = false ✓
  ✓ NO requiere Danado = true (es crédito)
  ✓ Stock NO cambia
  ✓ Proveedor.CreditoDisponible += TotalCredito
  ✓ ProductoPendiente.Estado = "Cancelado"
```

---

## 🗄️ Base de Datos (Cambios)

### Migración: `20260418173434_ExtenderComprasYDevoluciones`

**Cambios a DetalleCompra:**
```sql
ALTER TABLE DetalleCompra
ADD NumeroLote NVARCHAR(255) DEFAULT '',
ADD EsOfertado BIT DEFAULT 0;
```

**Nueva Tabla ProductoPendiente:**
```sql
CREATE TABLE ProductoPendiente (
    ProductoPendienteId INT PRIMARY KEY IDENTITY,
    ProductoId INT NOT NULL,
    CompraId INT NOT NULL,
    Cantidad INT NOT NULL,
    CantidadProcesada INT DEFAULT 0,
    FechaCompra DATETIME2,
    Estado NVARCHAR(50) DEFAULT 'Pendiente',
    EmpresaId INT DEFAULT 1,
    FOREIGN KEY (ProductoId) REFERENCES Producto(ProductoId),
    FOREIGN KEY (CompraId) REFERENCES Compra(CompraId)
);
```

---

## ⚙️ Uso en Vistas

### Crear Compra (Vista: Create.cshtml)
```html
<!-- Agregar en formulario de DetalleCompra -->
<label>¿Comprado en Oferta?</label>
<input type="checkbox" name="EsOfertado" />
```

### Ver Productos Pendientes (URL)
```
/ProductosPendientes/Index?estado=Pendiente
/ProductosPendientes/Index?estado=Procesado
/ProductosPendientes/Resumen
```

---

## 🧪 Testeo Recomendado

1. **Compra Normal:**
   - Crear compra → Verificar NumeroLote generado
   - Verificar ProductoPendiente creado con estado "Pendiente"
   - Verificar stock incrementado

2. **Compra en Oferta:**
   - Crear compra con EsOfertado=true
   - Intentar devolver → Debe fallar con mensaje claro

3. **Devolución Dañado:**
   - Marcar producto como Danado=true
   - Devolver → Debe reducir stock
   - Verificar ProductoPendiente en "Procesado"

4. **Devolución Crédito:**
   - Crear devolucion con EsCredito=true
   - Verificar CreditoDisponible del proveedor incrementado
   - Verificar ProductoPendiente en "Cancelado"

---

## 📝 Notas Importantes

✅ **Compatibilidad Mantenida:**
- Todo el código anterior sigue funcionando
- No se eliminó ningún campo o tabla
- Se agrega funcionalidad, no se reemplaza

⚠️ **Consideraciones:**
- NumeroLote se genera automáticamente, no requiere entrada del usuario
- EsOfertado debe ser marcado al crear la compra (no puede cambiar después)
- ProductosPendientes se crea automáticamente, no requiere acción manual
- Las migraciones ya están aplicadas a la base de datos

🔐 **Seguridad:**
- MultiEmpresa respetado en todas las operaciones
- Validación de rol (Admin) en todos los controladores
- Transacciones en operaciones críticas

---

## 📞 Soporte

Para agregar más validaciones o campos:
1. Extender `ValidadorDevoluciones` con nuevos métodos
2. Agregar campos a `ProductoPendiente` si es necesario
3. Crear migración correspondiente
4. Actualizar controladores según sea necesario

El sistema está diseñado para ser extensible sin romper la funcionalidad existente.
