# 🚀 CryptoHub Developer Log

---

# Day 09 – API Validation and Error Contracts

## 🎯 Sprint Goal

Mejorar el endpoint de CryptoHub separando la lógica de interpretación del `TradingPair` y estableciendo un contrato claro para los errores de la API.

---

## 🧩 TradingPairParser

Se creó:

```text
Models/TradingPairParser.cs
```

Su responsabilidad es convertir el símbolo recibido por HTTP:

```text
BTC-USDT
```

en nuestro modelo de dominio:

```csharp
new TradingPair("BTC", "USDT")
```

El parser se encarga de:

- Limpiar espacios.
- Convertir el símbolo a mayúsculas.
- Separar `BaseAsset` y `QuoteAsset`.
- Validar que existan exactamente dos partes.
- Validar que ninguno de los assets esté vacío.
- Crear únicamente un `TradingPair` válido.

Flujo:

```text
" btc-usdt "
      ↓
    Trim
      ↓
 ToUpperInvariant
      ↓
    Split
      ↓
  Validación
      ↓
TradingPair("BTC", "USDT")
```

---

## 🧠 Separación de responsabilidades

Antes, `Program.cs` realizaba directamente:

```text
recibir symbol
    ↓
Split
    ↓
validar
    ↓
crear TradingPair
```

Ahora:

```text
Program.cs
    ↓
TradingPairParser
    ↓
TradingPair
```

`Program.cs` se mantiene enfocado en HTTP y en coordinar las dependencias.

`TradingPairParser` no conoce HTTP ni códigos de estado.

---

## 🔌 Dependency Injection

Se registró el parser mediante:

```csharp
builder.Services.AddScoped<TradingPairParser>();
```

El endpoint recibe el parser mediante Dependency Injection.

Esto permite que la lógica de parsing permanezca fuera de `Program.cs`.

---

# ❌ Manejo de TradingPairs inválidos

Se probó:

```text
GET /market/BTC
```

El parser detecta que no existe el formato:

```text
BASE-QUOTE
```

y lanza:

```csharp
ArgumentException
```

Inicialmente el endpoint convertía esta excepción en:

```text
500 Internal Server Error
```

Se identificó que esto era incorrecto desde el punto de vista HTTP.

Una entrada inválida del cliente debe producir:

```text
400 Bad Request
```

---

# 🌐 Diferencia entre 400 y 500

## 400 Bad Request

Se utiliza cuando el cliente envía una petición inválida.

Ejemplo:

```text
/market/BTC
```

## 500 Internal Server Error

Se reserva para errores inesperados del servidor.

Esta distinción se incorporó al manejo de excepciones del endpoint.

---

# 📦 ApiError

Se creó:

```text
Models/ApiError.cs
```

como un `record`:

```csharp
public record ApiError(
    string Error,
    string Message);
```

Su función es representar los errores que CryptoHub expone públicamente mediante su API.

---

## 🧠 Error interno vs contrato de API

El parser puede producir una excepción de C#:

```csharp
ArgumentException
```

Pero el cliente no necesita conocer los detalles internos de C#.

Por ello `Program.cs` transforma la excepción en:

```text
ApiError
```

y después ASP.NET Core la serializa a JSON.

Flujo:

```text
ArgumentException
       ↓
Program.cs
       ↓
ApiError
       ↓
HTTP 400
       ↓
JSON
```

---

# 🧪 Prueba de petición inválida

Se probó:

```bash
curl -i http://localhost:5234/market/BTC
```

Resultado:

```text
HTTP/1.1 400 Bad Request
```

Respuesta:

```json
{
  "error": "invalid_trading_pair",
  "message": "El TradingPair debe tener el formato BASE-QUOTE."
}
```

Se eliminó del mensaje público el detalle técnico:

```text
(Parameter 'symbol')
```

porque pertenece a la implementación interna y no al contrato público de la API.

---

# 🧪 Prueba de petición válida

Se probó:

```text
http://localhost:5234/market/BTC-USDT
```

CryptoHub continuó obteniendo correctamente el precio real desde BingX Futures.

Ejemplo de respuesta:

```json
{
  "tradingPair": {
    "baseAsset": "BTC",
    "quoteAsset": "USDT"
  },
  "value": 64160.7,
  "timestamp": "2026-08-18T02:42:25.188602+00:00"
}
```

Resultado:

```text
HTTP 200 OK
```

---

# 🏆 Estado del Sprint 5

```text
TradingPairParser       ✅
Validación              ✅
Normalización           ✅
Dependency Injection    ✅
ApiError                ✅
HTTP 400                ✅
HTTP 200                ✅
Precio real             ✅
```

---

# 🧠 Conceptos aprendidos

- Parsing.
- Validación de entrada.
- `string.Trim()`.
- `ToUpperInvariant()`.
- `string.Split()`.
- `string.IsNullOrWhiteSpace()`.
- `ArgumentException`.
- Jerarquía de excepciones.
- Orden de los `catch`.
- HTTP 400 vs HTTP 500.
- Contratos de API.
- DTO/modelos de respuesta.
- Separación de responsabilidades.
- Dependency Injection.
- `record` para transportar información.

---

# 🏗️ Arquitectura actual

```text
Cliente
   ↓
Program.cs
   ↓
TradingPairParser
   ↓
TradingPair
   ↓
IExchange
   ↓
BingXFuturesExchange
   ↓
HttpClient
   ↓
BingX Futures
   ↓
BingX DTO
   ↓
Price
   ↓
JSON de CryptoHub
```

Para errores:

```text
Cliente
   ↓
Program.cs
   ↓
TradingPairParser
   ↓
ArgumentException
   ↓
ApiError
   ↓
HTTP 400
```

---

# 🚀 Próximo objetivo

Antes de seguir agregando funcionalidades, se diseñará el contrato definitivo de los endpoints de CryptoHub.

Actualmente:

```text
GET /market/BTC-USDT
```

Se analizará una estructura más estable y orientada a recursos, por ejemplo:

```text
GET /api/markets/BTC-USDT/price
```

La decisión será importante porque estos endpoints serán consumidos posteriormente por:

- La web de CryptoHub.
- La futura aplicación para iPhone.
- Otros clientes.

---

# 💭 Reflexión

CryptoHub ya no solamente obtiene información de BingX.

Ahora existe una separación clara entre:

```text
Entrada HTTP
    ↓
Parsing y validación
    ↓
Dominio
    ↓
Abstracción del exchange
    ↓
Proveedor externo
    ↓
Dominio
    ↓
Respuesta HTTP
```

La API empieza a tener sus propios contratos y reglas, independientemente de cómo BingX represente internamente sus datos.

---

# ☕ Quote

> "Una API no solo devuelve datos; también define cómo sus clientes entienden el éxito y el error."
