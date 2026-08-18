# 🚀 CryptoHub Developer Log

---

# Day 07 – Designing the Exchange Contract

## 🎯 Objetivo

Continuar la arquitectura de CryptoHub y convertir `IExchange` en el contrato que utilizarán los exchanges.

## ✅ Completado

- Se definió que CryptoHub comenzará implementando **Futures**.
- Se implementó `IExchange`.
- Se definió que un exchange debe poder obtener un `Price` a partir de un `TradingPair`.
- Se creó `BingXFuturesExchange`.
- Se decidió no acoplar el dominio directamente a BingX.

## 🏗️ Arquitectura

La arquitectura evolucionó de:

```text
Program.cs
    ↓
MarketService
    ↓
BingX
```

a:

```text
Program.cs
    ↓
IExchange
    ↓
BingXFuturesExchange
    ↓
BingX
```

## 🧠 Conceptos aprendidos

- Interfaces como contratos.
- Dependencia hacia abstracciones.
- Separación de responsabilidades.
- Implementación concreta de una interfaz.
- `Task<Price>`.
- `async` / `await`.
- `HttpClient`.

## 🔄 Decisión importante

`BingXExchange` era demasiado genérico para el diseño actual.

Se renombró a:

```text
BingXFuturesExchange
```

Esto deja preparada la arquitectura para agregar posteriormente:

```text
BingXSpotExchange
```

---

# Day 08 – BingX Futures: HTTP, DTO y Price

## 🎯 Sprint Goal

Conectar CryptoHub con BingX Futures y transformar la respuesta externa de BingX en nuestro modelo de dominio `Price`.

## 🔄 Flujo implementado

```text
GET /market/BTC-USDT
        ↓
Program.cs
        ↓
IExchange
        ↓
BingXFuturesExchange
        ↓
HttpClient
        ↓
BingX Futures API
        ↓
JSON
        ↓
BingX DTO
        ↓
decimal
        ↓
Price
        ↓
JSON de CryptoHub
```

## 🌐 Endpoint utilizado

```text
GET /openApi/swap/v2/quote/ticker
```

La URL se construye utilizando el `TradingPair`.

Ejemplo:

```text
BTC-USDT
```

## 🧩 DTOs

Se creó:

```text
Models
└── External
    └── BingX
        ├── BingXPriceResponse.cs
        └── BingXPriceData.cs
```

Los DTOs representan únicamente la estructura que BingX envía.

### `BingXPriceResponse`

Contiene:

```text
Code
Data
```

### `BingXPriceData`

Contiene:

```text
Symbol
LastPrice
```

## 🔤 JSON → C#

Se utilizó:

```csharp
System.Text.Json
```

y:

```csharp
JsonSerializer.Deserialize<T>()
```

También se aprendió a utilizar:

```csharp
[JsonPropertyName("lastPrice")]
```

para indicar explícitamente cómo se llama una propiedad en el JSON externo.

Esto resolvió la diferencia entre:

```text
lastPrice
```

y:

```text
LastPrice
```

## 🧠 DTO vs Domain Model

Se decidió no convertir directamente el JSON externo en `Price`.

El flujo es:

```text
JSON de BingX
      ↓
BingXPriceResponse
      ↓
Price
```

Esto mantiene el dominio de CryptoHub independiente de la estructura específica de BingX.

## 💰 Conversión del precio

BingX entrega `lastPrice` como texto.

Por ello:

```text
"64171.7"
      ↓
decimal
      ↓
Price.Value
```

## ⏱️ Timestamp

El `Price` recibe:

```csharp
DateTimeOffset.UtcNow
```

para registrar el instante en que CryptoHub obtuvo el precio.

## 🗑️ Eliminación de MarketTicker

Se eliminó:

```text
Models/MarketTicker.cs
```

para evitar dos modelos que representaran prácticamente el mismo concepto.

El dominio utiliza ahora:

```text
Price
```

como modelo principal para representar un precio.

---

# 🧪 Primera prueba real

Se probó:

```text
http://localhost:5234/market/BTC-USDT
```

Inicialmente se obtuvo:

```text
500
The method or operation is not implemented.
```

Esto confirmó que el endpoint estaba llegando correctamente a `BingXFuturesExchange`, pero el método todavía contenía:

```csharp
throw new NotImplementedException();
```

Después se completó el método.

## 🏆 Resultado

CryptoHub devolvió exitosamente:

```json
{
  "tradingPair": {
    "baseAsset": "BTC",
    "quoteAsset": "USDT"
  },
  "value": 64171.7,
  "timestamp": "2026-08-18T02:12:10.902157+00:00"
}
```

Esto confirmó el flujo completo:

```text
BingX
 ↓
JSON
 ↓
DTO
 ↓
decimal
 ↓
Price
 ↓
CryptoHub API
```

---

# 🔧 Refactorización de Program.cs

Se reemplazó la dependencia directa de `MarketService`.

Se registró:

```csharp
builder.Services.AddScoped<IExchange, BingXFuturesExchange>();
```

El endpoint pasó a recibir:

```csharp
IExchange exchange
```

ASP.NET Core resuelve automáticamente la implementación concreta mediante Dependency Injection.

## Arquitectura actual

```text
Cliente
   ↓
Program.cs
   ↓
IExchange
   ↓
BingXFuturesExchange
   ↓
HttpClient
   ↓
BingX Futures
```

---

# 🧹 Limpieza

Se eliminó `MarketTicker`.

También se comprobó que el proyecto compilara correctamente.

La única advertencia restante corresponde al paquete:

```text
Microsoft.OpenApi 2.0.0
```

por una vulnerabilidad conocida. Esta advertencia es independiente del Sprint actual.

---

# 📦 Git

Se realizó commit y push del Sprint.

Commit:

```text
feat: integrate BingX futures price
```

El repositorio quedó sincronizado con:

```text
origin/main
```

---

# 🧠 Principales aprendizajes

- Interfaces como contratos.
- Dependency Injection.
- `IExchange` como abstracción.
- Implementaciones específicas por tipo de mercado.
- `HttpClient`.
- `HttpResponseMessage`.
- `EnsureSuccessStatusCode()`.
- `ReadAsStringAsync()`.
- DTOs.
- `System.Text.Json`.
- `JsonSerializer.Deserialize<T>()`.
- `[JsonPropertyName]`.
- Conversión de `string` a `decimal`.
- Separación entre DTO y modelo de dominio.
- Refactorización.
- Eliminación de modelos duplicados.
- Pruebas mediante endpoints reales.
- Conventional Commits.

---

# 🏆 Estado actual de CryptoHub

```text
Fundamentos                 ✅
Git / GitHub                ✅
ASP.NET Core                ✅
Web API                     ✅
TradingPair                 ✅
Price                       ✅
IExchange                   ✅
Dependency Injection        ✅
BingXFuturesExchange        ✅
HTTP con BingX              ✅
DTOs                        ✅
JSON → Price                ✅
Primer precio real          ✅
MarketTicker                ❌ Eliminado
```

---

# 🚀 Próximo Sprint

## Sprint 5 – API de CryptoHub

Objetivo:

Diseñar el primer endpoint oficial de precios de CryptoHub, pensado desde el principio para ser consumido posteriormente por:

- La web de CryptoHub.
- La futura aplicación para iPhone.
- Otros clientes.

La idea será evolucionar el endpoint actual hacia una API más limpia y estable, manteniendo el dominio independiente de BingX.

---

# 💭 Reflexión

CryptoHub ya no solamente está consultando una API externa.

Ahora existe una separación clara entre:

```text
Proveedor externo
      ↓
     DTO
      ↓
Dominio de CryptoHub
      ↓
API de CryptoHub
```

Esto representa un cambio importante: CryptoHub empieza a convertirse en una plataforma propia, en lugar de ser simplemente un cliente de BingX.

---

# ☕ Quote

> "No estamos aprendiendo C#. Estamos construyendo CryptoHub."
