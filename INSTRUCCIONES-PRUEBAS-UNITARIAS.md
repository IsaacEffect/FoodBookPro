# Instrucciones: Pruebas unitarias en Visual Studio 2022

## 1. Abrir el proyecto

1. Abre Visual Studio 2022.
2. **Archivo** > **Abrir** > **Proyecto o solución**.
3. Navega a: `C:\Users\luisj\Source\Repos\FoodBookPro`
4. Selecciona `FoodBookPro.sln`.

---

## 2. Ubicar las pruebas

En el **Explorador de soluciones** (panel derecho):

- Expande **FoodBookPro.Test**
- Verás estas clases de prueba:
  - `Models/CartItemViewModelTests.cs`
  - `Models/CartViewModelTests.cs`
  - `Entities/EstadoOrdenTests.cs`
  - `Controllers/OrdersControllerTests.cs`

---

## 3. Ejecutar todas las pruebas

**Opción A – Explorador de pruebas:**
1. Menú **Prueba** > **Explorador de pruebas** (o `Ctrl+E, T`).
2. Clic en **Ejecutar todas las pruebas** (icono ▶).
3. Revisa los resultados: deben aparecer en verde.

**Opción B – Desde un archivo de pruebas:**
1. Abre un archivo `*Tests.cs`.
2. Clic derecho sobre la clase o un método `[Fact]`.
3. Elige **Ejecutar pruebas**.

---

## 4. Ejecutar una sola prueba

1. Abre un archivo de pruebas.
2. Junto a un método `[Fact]` o `[Theory]` verás un icono de ejecutar.
3. Clic en ese icono para ejecutar solo esa prueba.

---

## 5. Ver resultados

- **Verde**: prueba aprobada.
- **Rojo**: prueba fallida (mensaje de error y stack trace).
- **Amarillo**: prueba omitida.

---

## 6. Desde la terminal (PowerShell)

```powershell
cd C:\Users\luisj\Source\Repos\FoodBookPro
dotnet test
```

---

## Pruebas incluidas

| Archivo | Qué prueba |
|---------|------------|
| CartItemViewModelTests | Cálculo de Subtotal |
| CartViewModelTests | Cálculo de Total |
| EstadoOrdenTests | Valores del enum |
| OrdersControllerTests | Index, Detail, UpdateStatus, filtros |

**Total:** 17 pruebas.
