# GUÍA RÁPIDA: Extensión de Compras y Devoluciones

## 🎯 Objetivos Logrados

✅ **Rastreo de Lotes por Proveedor**
- Cada línea de compra genera un `NumeroLote` único
- Formato: `LOTE-{ProveedorId}-{CompraId}-{Timestamp}`
- Se pueden devolver/procesar lotes específicos

✅ **Control de Ofertas en Compra**
- Campo `EsOfertado` en cada línea de `DetalleCompra`
- Bloquea automáticamente devoluciones de productos en oferta
- Mensajes claros de error

✅ **Validaciones Estrictas de Devolución**
- ❌ NO se devuelven si fueron comprados en oferta (`EsOfertado = false` requerido)
- ❌ SOLO se devuelven si están dañados (`Danado = true` requerido)
- ❌ Límite de 7 días desde compra
- ✅ Excepción: Notas de crédito no requieren estar dañados

✅ **Seguimiento de Productos Pendientes**
- Nueva tabla `ProductoPendiente` con estados
- Rastreo automático con cada compra
- Actualización automática con cada devolución

✅ **Notas de Crédito Mejoradas**
- Generan saldo a favor del proveedor
- Visible en `Proveedor.CreditoDisponible`
- Registro en `ProductosPendientes` con estado "Cancelado"

---

## 🚀 Cómo Usar

### 1️⃣ Crear una Compra Normal

```
Admin → Compras → Nueva Compra
├─ Proveedor: Seleccionar
├─ Productos:
│  └─ Producto: Seleccionar
│     Cantidad: 10
│     Precio: Q15.00
│     ¿Oferta?: NO ← IMPORTANTE
├─ Costo Envío: Q50
└─ Guardar
```

**Resultado:**
- ✅ Compra creada con ID (ej: #123)
- ✅ NumeroLote generado para cada línea
- ✅ Stock incrementado (+10 unidades)
- ✅ ProductoPendiente creado con estado "Pendiente"

---

### 2️⃣ Crear una Compra en Oferta

```
Admin → Compras → Nueva Compra
├─ Proveedor: Seleccionar
├─ Productos:
│  └─ Producto: Seleccionar
│     Cantidad: 5
│     Precio: Q10.00
│     ¿Oferta?: SÍ ← MARCA ESTE CHECKBOX
├─ Costo Envío: Q0
└─ Guardar
```

**Resultado:**
- ✅ Compra creada
- ✅ `EsOfertado = true` guardado
- ✅ Stock incrementado
- ✅ ProductoPendiente creado

**Intento de Devolución:**
```
❌ BLOQUEADO: "No se permite devolución de productos comprados en oferta"
```

---

### 3️⃣ Devolver Producto Dañado

```
Admin → Devoluciones → Nueva Devolución
├─ Compra: Seleccionar (#123)
├─ Motivo: "Producto llegó roto"
├─ ¿Es Nota de Crédito?: NO
├─ Productos:
│  └─ Producto: Seleccionar (del mismo que fue comprado)
│     Cantidad: 3
│     ¿Dañado?: SÍ ← MARCA ESTE CHECKBOX
└─ Guardar
```

**Validaciones Ejecutadas:**
```
1. ✓ ¿EsOfertado = false? → SÍ, permitido
2. ✓ ¿Danado = true? → SÍ, permitido
3. ✓ ¿Menos de 7 días? → SÍ, permitido
```

**Resultado:**
- ✅ Devolución creada
- ✅ Stock reducido (-3 unidades)
- ✅ ProductoPendiente actualizado a "Procesado"
- ✅ DetalleDevolucion registrado

---

### 4️⃣ Devolución por Nota de Crédito

```
Admin → Devoluciones → Nueva Devolución
├─ Compra: Seleccionar (#125)
├─ Motivo: "Problema con factura"
├─ ¿Es Nota de Crédito?: SÍ ← MARCA ESTE CHECKBOX
├─ Total de Crédito: Q150.00
├─ Productos:
│  └─ Producto: Seleccionar
│     Cantidad: 5
│     ¿Dañado?: NO ← NO REQUIERE
└─ Guardar
```

**Resultado:**
- ✅ Devolución por crédito registrada
- ✅ `Proveedor.CreditoDisponible += Q150.00`
- ✅ Stock NO se modifica
- ✅ ProductoPendiente actualizado a "Cancelado"
- ✅ Crédito disponible para futuras compras

---

### 5️⃣ Visualizar Productos Pendientes

```
Admin → Productos Pendientes → Index

Filtros disponibles:
├─ ⏳ Pendiente → Productos no procesados
├─ ✅ Procesado → Productos devueltos
├─ ❌ Cancelado → Productos con crédito
└─ 📊 Resumen → Dashboard general
```

**Dashboard muestra:**
- Total de registros por estado
- Total de unidades por estado
- Porcentaje de procesamiento
- Información de cada producto

---

## 📋 Cambios en la UI

### Vista: Compras/Create.cshtml
```html
<!-- NUEVO CAMPO -->
<div class="col">
    <div class="form-check">
        <input type="checkbox" id="esOfertado" />
        <label for="esOfertado">¿Oferta?</label>
    </div>
</div>
```

### Vista: Devoluciones/Create.cshtml
```html
<!-- NUEVA SECCIÓN INFORMATIVA -->
<div class="alert alert-info">
    <h5>⚠️ Reglas de Devolución</h5>
    <ul>
        <li>✅ Solo productos NO comprados en oferta</li>
        <li>✅ Debe estar marcado como dañado</li>
        <li>✅ Máximo 7 días desde compra</li>
    </ul>
</div>
```

### Nueva Sección de Navegación
```
Menú Admin:
├─ Compras
├─ Devoluciones
├─ Facturas
└─ 📦 Productos Pendientes ← NUEVA
   ├─ Ver Pendientes
   ├─ Ver Procesados
   ├─ Ver Cancelados
   └─ Resumen
```

---

## 🧪 Casos de Prueba

### Test 1: Compra y Devolución Normal
```
1. Crear compra: 10 unidades @ Q20, NO oferta
2. Verificar: Stock = +10
3. Crear devolución: 3 unidades, dañado = true
4. Verificar: Stock = +7 (original 10 - devuelto 3)
5. Verificar: ProductoPendiente estado = "Procesado"
```

### Test 2: Oferta Bloqueada
```
1. Crear compra: 5 unidades @ Q15, SÍ oferta
2. Verificar: EsOfertado = true
3. Intentar devolución
4. Verificar: Bloqueado con mensaje claro
```

### Test 3: Crédito de Proveedor
```
1. Proveedor inicial: CreditoDisponible = 0
2. Crear devolución por crédito: Q100
3. Verificar: Proveedor.CreditoDisponible = 100
4. Crear compra nueva: Q80
5. Verificar: Crédito aplicado automáticamente
6. Verificar: Compra.Total = Q0 (pagada con crédito)
```

---

## 🔧 Extensibilidad

El sistema está diseñado para ser extensible:

### Agregar Nueva Validación
```csharp
// En ValidadorDevoluciones.cs
public (bool esValido, List<string> errores) Validar(...)
{
    var errores = new List<string>();
    
    // Agregar nueva validación
    if (condición) {
        errores.Add("Nuevo mensaje de error");
    }
    
    return (errores.Count == 0, errores);
}
```

### Agregar Nuevo Campo a ProductoPendiente
```csharp
// En Models/ProductoPendiente.cs
public class ProductoPendiente
{
    // ... campos existentes ...
    
    // NUEVO CAMPO
    public string CodigoLote { get; set; } = "";
}
```

### Crear Migración
```powershell
dotnet ef migrations add AgregarCampoProductoPendiente
dotnet ef database update
```

---

## 📊 Base de Datos

### Cambios Realizados

**Tabla: DetalleCompra**
```sql
ALTER TABLE DetalleCompra ADD NumeroLote NVARCHAR(255);
ALTER TABLE DetalleCompra ADD EsOfertado BIT DEFAULT 0;
```

**Tabla Nueva: ProductoPendiente**
```sql
CREATE TABLE ProductoPendiente (
    ProductoPendienteId INT PRIMARY KEY IDENTITY,
    ProductoId INT NOT NULL,
    CompraId INT NOT NULL,
    Cantidad INT NOT NULL,
    CantidadProcesada INT DEFAULT 0,
    FechaCompra DATETIME2,
    Estado NVARCHAR(50),
    EmpresaId INT,
    FOREIGN KEY (ProductoId) REFERENCES Producto,
    FOREIGN KEY (CompraId) REFERENCES Compra
);
```

---

## 🎓 Conceptos Importantes

### NumeroLote
- Único por línea de compra
- Permite identificar exactamente qué se compró
- Formato: `LOTE-{ProveedorId}-{CompraId}-{Timestamp}`
- Inmutable después de creado

### EsOfertado
- Marcado al crear la compra
- NO se puede cambiar después
- Bloquea TODAS las devoluciones (excepto crédito)
- Aplica POR LÍNEA, no al producto completo

### ProductoPendiente
- Se crea automáticamente con cada compra
- Se actualiza automáticamente con devoluciones
- Estados: Pendiente → Procesado o Cancelado
- Útil para auditoría y seguimiento

### Validaciones en Cascada
```
Devolución solicitada
├─ ¿EsOfertado = true? → BLOQUEADO
├─ ¿EsDañado = false? → BLOQUEADO (solo para no-crédito)
├─ ¿Más de 7 días? → BLOQUEADO
└─ ¿Producto en oferta (catálogo)? → BLOQUEADO
```

---

## 🆘 Troubleshooting

### Problema: "No se permite devolución de productos comprados en oferta"

**Causa:** El producto tiene `EsOfertado = true` en la compra original

**Solución:**
- Verificar que el checkbox "¿Oferta?" NO estaba marcado
- Si fue error, crear nota de crédito en lugar de devolución

### Problema: "Solo se pueden devolver productos marcados como dañados"

**Causa:** El producto no tiene `Danado = true` en la devolución

**Solución:**
- Marcar el checkbox "¿Dañado?" si realmente está dañado
- Si no está dañado, crear devolución por nota de crédito

### Problema: "No se permite devolución después de 7 días"

**Causa:** Más de 7 días desde la compra original

**Solución:**
- Contactar administrador si hay excepción
- Crear nota de crédito si aplica

---

## 📞 Preguntas Frecuentes

**¿Puedo cambiar EsOfertado después de guardar?**
- ❌ No, se genera al crear la compra y es inmutable

**¿Las notas de crédito requieren producto dañado?**
- ❌ No, las notas de crédito son una excepción

**¿El stock se reduce con devoluciones por crédito?**
- ❌ No, solo con devoluciones de productos dañados

**¿Puedo devolver 7 días exactos después?**
- ✅ Sí, el límite es ESTRICTO: hasta las 23:59 del día 7

**¿Los ProductosPendientes son solo informativos?**
- ❌ No, se actualizan automáticamente y se pueden cambiar manualmente

---

## 📝 Próximos Pasos Opcionales

1. **Reportes:**
   - Reporte de devoluciones por proveedor
   - Reporte de productos en oferta

2. **Notificaciones:**
   - Alertar cuando ProductoPendiente vencen 7 días

3. **Análisis:**
   - Estadísticas de devoluciones
   - Proveedores con más devoluciones

4. **Mejoras:**
   - API para consultar disponibilidad por lote
   - Exportar productos pendientes a Excel
