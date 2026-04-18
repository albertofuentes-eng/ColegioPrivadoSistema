# 🗺️ MAPA DE NAVEGACIÓN - MÓDULO DE COMPRAS Y DEVOLUCIONES

## Accesos Rápidos

### 📋 Panel Principal
```
Home (/Home/Index)
├─ Admin Panel
│  ├─ Compras
│  ├─ Devoluciones
│  └─ 📦 Productos Pendientes ← NUEVO
│     ├─ Ver Pendientes
│     ├─ Ver Procesados
│     ├─ Ver Cancelados
│     └─ Resumen
├─ Facturas
├─ Productos
├─ Proveedores
└─ Usuarios
```

---

## 🔗 URLs Disponibles

### Compras
```
GET  /Compras/Index                → Listar compras
GET  /Compras/Create               → Crear nueva compra
POST /Compras/Create               → Guardar compra
     [Incluye: NumeroLote, EsOfertado]
```

### Devoluciones
```
GET  /Devoluciones/Index           → Listar devoluciones
GET  /Devoluciones/Create          → Crear devolución
POST /Devoluciones/Create          → Guardar devolución
     [Validaciones mejoradas]
```

### Productos Pendientes (NUEVO)
```
GET  /ProductosPendientes/Index?estado=Pendiente     → Pendientes
GET  /ProductosPendientes/Index?estado=Procesado     → Procesados
GET  /ProductosPendientes/Index?estado=Cancelado     → Cancelados
GET  /ProductosPendientes/Resumen                    → Dashboard
GET  /ProductosPendientes/Detalles/{id}              → Ver detalles
POST /ProductosPendientes/ActualizarEstado           → Cambiar estado
```

---

## 📱 Interfaz de Usuario

### Crear Compra
```
Formulario:
├─ Proveedor: [Dropdown]
├─ Costo Envío: [Input numérico]
├─
├─ Tabla de Productos:
│  ├─ Producto: [Dropdown]
│  ├─ Cantidad: [Input]
│  ├─ Precio Unitario: [Input]
│  ├─ ¿Oferta?: [Checkbox] ← NUEVO
│  └─ [Botón Agregar]
│
└─ [Botón Guardar Compra]
```

### Crear Devolución
```
Formulario:
├─ Compra: [Dropdown]
├─ Motivo: [Input texto]
├─ ¿Es Nota de Crédito?: [Checkbox]
├─ Total Crédito: [Input numérico - aparece si está marcado]
│
├─ ⚠️ Información de Reglas (NUEVO):
│  ├─ ✅ Solo NO ofertados
│  ├─ ✅ Deben estar dañados
│  ├─ ✅ Máximo 7 días
│  └─ ✅ Excepto notas de crédito
│
├─ Tabla de Productos:
│  ├─ Producto: [Dropdown]
│  ├─ Cantidad: [Input]
│  ├─ ¿Dañado?: [Checkbox]
│  └─ [Botón Agregar]
│
└─ [Botón Guardar Devolución]
```

### Productos Pendientes - Resumen
```
Cards Estadísticas:
├─ ⏳ Pendiente
│  ├─ {N} productos
│  └─ {M} unidades
│
├─ ✅ Procesado
│  ├─ {N} productos
│  └─ {M} unidades
│
└─ ❌ Cancelado
   ├─ {N} productos
   └─ {M} unidades

Tabla Filtrable:
├─ Botones: Pendiente | Procesado | Cancelado | Resumen
└─ Tabla con detalles
```

---

## 🔐 Roles y Permisos

```
Admin:
├─ ✅ Crear Compras
├─ ✅ Crear Devoluciones
├─ ✅ Ver Productos Pendientes
├─ ✅ Cambiar Estado de Pendientes
└─ ✅ Acceso a todas las funciones

No-Admin:
└─ ❌ Redirigido a Home
```

---

## 🔍 Filtros y Búsquedas

### Productos Pendientes
```
Estados Disponibles:
├─ Pendiente      (Sin procesar)
├─ Procesado      (Devuelto)
└─ Cancelado      (Crédito)

Ordenamiento:
└─ Por Fecha de Compra (ascendente)
```

---

## 📊 Datos Mostrados

### Compra
```
✓ ID de Compra
✓ Fecha
✓ Proveedor
✓ Total
✓ Costo Envío
✓ Productos relacionados
```

### Devolución  
```
✓ ID de Devolución
✓ Compra Relacionada
✓ Fecha
✓ Motivo
✓ Tipo (Normal / Crédito)
✓ Total Crédito (si aplica)
✓ Productos devueltos
```

### Producto Pendiente
```
✓ ID Registro
✓ Producto
✓ Compra Asociada
✓ Cantidad Total
✓ Cantidad Procesada
✓ Porcentaje de Progreso
✓ Fecha de Compra
✓ Estado (Pendiente/Procesado/Cancelado)
✓ Acciones (Ver, Cambiar Estado)
```

---

## ⚡ Acciones Rápidas

### Desde Index de Compras
```
[Ver] → Detalles de compra
[Editar] → (Si aplica)
[Devolver] → Ir a crear devolución
[Eliminar] → (Si aplica)
```

### Desde Index de Devoluciones
```
[Ver] → Detalles de devolución
[Editar] → (Si aplica)
[Anular] → (Si aplica)
```

### Desde Index de Productos Pendientes
```
[Ver] → Ir a Detalles
[Cambiar Estado] → Modal/Formulario
[Filtrar] → Por estado
[Buscar] → Por producto (si aplica)
```

---

## 🎯 Flujos de Usuario

### Flujo: Comprar Producto Normal
```
1. Admin → Compras → Nueva Compra
2. Selecciona Proveedor
3. Agrega Producto (¿Oferta? = NO)
4. Confirma
5. Sistema crea ProductoPendiente
6. Vista: ProductosPendientes → Ve "Pendiente"
```

### Flujo: Comprar Producto en Oferta
```
1. Admin → Compras → Nueva Compra
2. Selecciona Proveedor
3. Agrega Producto (¿Oferta? = SÍ)
4. Confirma
5. Sistema crea ProductoPendiente
6. Vista: ProductosPendientes → Ve "Pendiente" (con oferta marcada)
```

### Flujo: Devolver Producto Dañado
```
1. Admin → Devoluciones → Nueva Devolución
2. Selecciona Compra
3. Agrega Producto (¿Dañado? = SÍ)
4. NO marca "Es Nota de Crédito"
5. Sistema valida: NO ofertado ✓, Dañado ✓
6. Confirma → Se reduce stock
7. ProductoPendiente → Estado "Procesado"
```

### Flujo: Devolución por Crédito
```
1. Admin → Devoluciones → Nueva Devolución
2. Selecciona Compra
3. Marca "Es Nota de Crédito"
4. Ingresa Total Crédito
5. Agrega Productos (¿Dañado? = NO, no importa)
6. Sistema valida: NO ofertado ✓
7. Confirma → Saldo a favor incrementado
8. ProductoPendiente → Estado "Cancelado"
```

---

## 📈 Seguimiento Visual

### Barra de Progreso (Detalles de ProductoPendiente)
```
Comprado: 10 unidades
Procesado: 3 unidades (por devolución)
Pendiente: 7 unidades

Barra:  [████░░░░░░░░░░░░░░░░] 30%
```

---

## 🔄 Integración con Otros Módulos

### Productos
```
Producto → Vista:
├─ Stock (afectado por Compras y Devoluciones)
├─ EnOferta (validación en Devoluciones)
├─ CodigoLote (generado en Compra)
└─ PrecioCompra / PrecioVenta
```

### Proveedores
```
Proveedor → Vista:
├─ CreditoDisponible (afectado por Notas de Crédito)
└─ Detalles en cada Compra
```

### Facturas
```
Factura → Relacionada a:
└─ Puede estar vinculada a Compras
```

---

## 🎨 Estilos y Badges

### Estados
```
Pendiente      → Badge: bg-warning (Amarillo)
Procesado      → Badge: bg-success (Verde)
Cancelado      → Badge: bg-danger (Rojo)
```

### Acciones
```
Ver            → btn-info (Azul)
Guardar        → btn-primary (Azul oscuro)
Agregar        → btn-success (Verde)
Eliminar       → btn-danger (Rojo)
Volver         → btn-secondary (Gris)
```

---

## 📱 Responsividad

```
✓ Desktop: Vista completa
✓ Tablet: Tabla con scroll horizontal
✓ Mobile: Stack vertical, botones adaptados
```

---

## ⌨️ Atajos de Teclado (Futuros)

```
Ctrl + N  → Nueva Compra
Ctrl + L  → Ir a Compras
Ctrl + D  → Ir a Devoluciones
Ctrl + P  → Ir a Productos Pendientes
```

---

## 🔔 Notificaciones

### Sistema de Mensajes
```
✅ Verde: Operación exitosa
❌ Rojo: Error / Validación fallida
⚠️ Amarillo: Advertencia
ℹ️ Azul: Información
```

### Ejemplos
```
✅ "Compra #123 creada exitosamente"
❌ "No se permite devolución de productos comprados en oferta"
⚠️ "Stock bajo para producto: Tornillos"
ℹ️ "Crédito disponible: Q500.00"
```

---

## 🖨️ Reportes (Futuros)

```
Disponibles en ProductosPendientes:
├─ Imprimir Resumen
├─ Descargar Excel
├─ Enviar Email
└─ Generar PDF
```

---

## 💡 Tips y Trucos

1. **Búsqueda rápida:** Usa Ctrl+F en Index
2. **Múltiples productos:** Agrega varios antes de guardar
3. **Correcciones:** Las compras guardadas NO pueden editarse, solo crear notas de crédito
4. **Stock:** Verifica en Productos si se actualizó correctamente
5. **Auditoría:** Usa NumeroLote para rastrear historial

---

## ✨ Características Futuras Sugeridas

- [ ] Exportar a Excel
- [ ] Generador de códigos de barras
- [ ] Notificaciones por email
- [ ] Dashboard gráfico
- [ ] API REST
- [ ] Búsqueda avanzada
- [ ] Historial de cambios
- [ ] Comparativa con presupuesto
