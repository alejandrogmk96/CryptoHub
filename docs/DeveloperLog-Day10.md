# 🚀 CryptoHub Developer Log

---

# Day 10 – Market Candles and Public Market Data

## 🎯 Sprint Goal

Continuar la construcción de la API pública de CryptoHub y agregar soporte real para velas de mercado (candles) utilizando BingX Futures.

El objetivo fue que CryptoHub pudiera recibir datos externos de BingX, transformarlos al dominio propio de CryptoHub y exponerlos mediante un contrato de API independiente del proveedor.

---

# 🧩 Situación inicial

Al comenzar esta sesión, CryptoHub ya contaba con:

- `TradingPair`
- `Price`
- `IExchange`
- `BingXFuturesExchange`
- `TradingPairParser`
- `ApiError`
- Endpoint público de precio.

La arquitectura ya separaba el dominio de CryptoHub de la estructura específica de BingX.

Arquitectura:

```text
Cliente
   ↓
Market Endpoint
   ↓
TradingPairParser
   ↓
IExchange
   ↓
BingXFuturesExchange
   ↓
BingX Futures
```

---

# 📈 Candle

Se completó el modelo de dominio:

```text
Models/Candle.cs
```

Una vela representa información OHLCV de un `TradingPair` en un instante determinado.

Propiedades utilizadas:

```text
TradingPair
Open
High
Low
Close
Volume
Timestamp
```

Para valores financieros se utiliza `decimal`.

Para el instante temporal se utiliza `DateTimeOffset`.

---

# ⏱️ CandleInterval

Se agregó el concepto de intervalo de vela:

```text
CandleInterval
```

Los intervalos soportados actualmente son:

```text
OneMinute
FiveMinutes
FifteenMinutes
OneHour
FourHours
OneDay
```

Estos valores pertenecen al dominio de CryptoHub y no dependen directamente de BingX.

---

# 🔌 IExchange

Se amplió el contrato de `IExchange`.

Ahora un exchange debe poder:

```text
GetPriceAsync()
GetCandlesAsync()
```

Esto mantiene la arquitectura preparada para soportar diferentes exchanges sin modificar el dominio.

Arquitectura:

```text
IExchange
   │
   ├── GetPriceAsync()
   │
   └── GetCandlesAsync()
```

---

# 🏦 BingXFuturesExchange

Se agregó soporte de candles a:

```text
Services/BingXFuturesExchange.cs
```

Se agregó el endpoint externo de BingX:

```text
/openApi/swap/v3/quote/klines
```

La URL se construye utilizando:

```text
symbol
interval
limit
```

Ejemplo:

```text
BTC-USDT
1m
10
```

---

# 🧩 BingXCandle

Se creó:

```text
Models/External/BingX/BingXCandle.cs
```

Durante la implementación se descubrió mediante una petición real que BingX devuelve las velas como objetos JSON y no como arrays posicionales.

La estructura real observada fue:

```json
{
  "open": "64122.0",
  "close": "64114.7",
  "high": "64122.0",
  "low": "64114.7",
  "volume": "1.0111",
  "time": 1787031480000
}
```

El modelo externo refleja únicamente la estructura enviada por BingX.

---

# 📦 BingXCandleResponse

Se creó:

```text
Models/External/BingX/BingXCandleResponse.cs
```

Representa la respuesta completa de BingX:

```text
Code
Msg
Data
```

`Data` contiene una lista de `BingXCandle`.

Se utilizaron atributos:

```csharp
[JsonPropertyName(...)]
```

para mapear explícitamente los nombres del JSON externo.

---

# 🔄 BingXCandleMapper

Se creó:

```text
Services/BingXCandleMapper.cs
```

Su única responsabilidad es transformar:

```text
BingXCandle
      ↓
Candle
```

Las conversiones realizadas son:

```text
open   → Open
high   → High
low    → Low
close  → Close
volume → Volume
time   → Timestamp
```

Los valores numéricos recibidos como texto son convertidos a `decimal` utilizando:

```text
CultureInfo.InvariantCulture
```

El timestamp de BingX se recibe como Unix Time en milisegundos y se transforma mediante:

```csharp
DateTimeOffset.FromUnixTimeMilliseconds(...)
```

---

# 🔢 BingXCandleIntervalMapper

Se creó:

```text
Services/BingXCandleIntervalMapper.cs
```

Su responsabilidad es traducir el intervalo interno de CryptoHub al formato requerido por BingX.

Conversión:

```text
OneMinute      → 1m
FiveMinutes    → 5m
FifteenMinutes → 15m
OneHour        → 1h
FourHours      → 4h
OneDay         → 1d
```

Esto mantiene al dominio de CryptoHub independiente del formato utilizado por BingX.

---

# 🌐 CandleIntervalMapper

También se creó:

```text
Services/CandleIntervalMapper.cs
```

Este mapper representa el intervalo en el contrato público de CryptoHub.

Ejemplo:

```text
CandleInterval.OneMinute
        ↓
      "1m"
```

De esta forma la API pública no expone los valores numéricos del enum.

---

# 📋 CandleResponse

Se creó:

```text
Models/CandleResponse.cs
```

La respuesta pública contiene:

```text
TradingPair
Interval
Candles
```

Ejemplo:

```json
{
  "tradingPair": {
    "baseAsset": "BTC",
    "quoteAsset": "USDT"
  },
  "interval": "1m",
  "candles": []
}
```

Esto establece un contrato propio de CryptoHub.

---

# 🌐 Endpoint público de candles

Se agregó:

```text
GET /api/markets/{symbol}/candles
```

Ejemplo:

```text
GET /api/markets/BTC-USDT/candles?interval=OneMinute&limit=10
```

El endpoint realiza:

```text
HTTP
 ↓
TradingPairParser
 ↓
TradingPair
 ↓
CandleInterval
 ↓
IExchange
 ↓
BingXFuturesExchange
 ↓
BingX
 ↓
BingXCandleResponse
 ↓
BingXCandleMapper
 ↓
Candle
 ↓
CandleResponse
 ↓
JSON
```

---

# 🧪 Prueba real contra BingX

Antes de completar la deserialización se realizó una prueba directa utilizando:

```bash
curl "https://open-api.bingx.com/openApi/swap/v3/quote/klines?symbol=BTC-USDT&interval=1m&limit=10"
```

La respuesta confirmó la estructura real de BingX y permitió corregir el modelo `BingXCandle`.

Este fue un aprendizaje importante:

> No debemos asumir la estructura de una API externa. Debemos comprobar la respuesta real antes de diseñar los DTOs.

---

# 🐛 Bug del día

Inicialmente `BingXCandle` fue diseñado pensando que BingX enviaba las velas como arrays.

Se esperaba algo conceptualmente similar a:

```text
[time, open, high, low, close, volume]
```

Sin embargo, la respuesta real fue:

```json
{
  "open": "...",
  "close": "...",
  "high": "...",
  "low": "...",
  "volume": "...",
  "time": 123456789
}
```

Se corrigió el DTO para representar la estructura real.

---

# 🐛 Segundo problema encontrado

Después de corregir `BingXCandle`, la API inicialmente devolvía:

```json
[]
```

La causa fue que `System.Text.Json` no estaba asignando automáticamente las propiedades:

```text
data
open
close
high
low
volume
time
```

a las propiedades C# correspondientes.

Se resolvió utilizando:

```csharp
[JsonPropertyName("data")]
[JsonPropertyName("open")]
[JsonPropertyName("close")]
[JsonPropertyName("high")]
[JsonPropertyName("low")]
[JsonPropertyName("volume")]
[JsonPropertyName("time")]
```

Después de la corrección, las velas comenzaron a aparecer correctamente.

---

# 🔄 Orden de las velas

BingX entrega las velas de la más reciente a la más antigua.

Ejemplo recibido:

```text
05:53
05:52
05:51
...
05:44
```

CryptoHub ahora las ordena mediante:

```csharp
.OrderBy(candle => candle.Timestamp)
```

Por lo tanto, la API pública entrega:

```text
05:44
05:45
05:46
...
05:53
```

Esto deja los datos preparados para alimentar posteriormente una gráfica.

---

# 🧪 Pruebas realizadas

## BTC-USDT — 1 minuto

Request:

```text
/api/markets/BTC-USDT/candles?interval=OneMinute&limit=10
```

Resultado:

```text
HTTP 200
```

Respuesta:

```text
interval: "1m"
```

Las 10 velas fueron recibidas y transformadas correctamente.

---

## BTC-USDT — 5 minutos

Request:

```text
/api/markets/BTC-USDT/candles?interval=FiveMinutes&limit=10
```

Resultado:

```text
HTTP 200
```

Respuesta:

```text
interval: "5m"
```

Las velas fueron recibidas correctamente y quedaron ordenadas cronológicamente.

---

# 🏗️ Arquitectura actual

```text
                     Cliente
                        │
                        ▼
               MarketEndpoints
                        │
                        ▼
              TradingPairParser
                        │
                        ▼
                  TradingPair
                        │
                        ▼
                   IExchange
                        │
                        ▼
             BingXFuturesExchange
                        │
                        ▼
                    HttpClient
                        │
                        ▼
                 BingX Futures
                        │
                        ▼
              BingXCandleResponse
                        │
                        ▼
                  BingXCandle
                        │
                        ▼
              BingXCandleMapper
                        │
                        ▼
                     Candle
                        │
                        ▼
                 CandleResponse
                        │
                        ▼
                CryptoHub JSON
```

---

# 🧠 Principales aprendizajes

Durante esta sesión se reforzaron:

- DTOs externos.
- Separación entre DTO y dominio.
- `JsonSerializer.Deserialize<T>()`.
- `JsonPropertyName`.
- `decimal` para datos financieros.
- Unix Time en milisegundos.
- `DateTimeOffset`.
- LINQ.
- `.Select()`.
- `.OrderBy()`.
- Mappers.
- Contratos de API.
- Abstracciones mediante interfaces.
- Diferencia entre modelo externo y modelo de dominio.
- Verificación de APIs mediante `curl`.
- Diseño de APIs independientes del proveedor.

---

# 🏆 Estado actual de CryptoHub

```text
Fundamentos                  ✅
Git / GitHub                 ✅
ASP.NET Core                 ✅
Web API                      ✅
TradingPair                  ✅
TradingPairParser            ✅
Price                        ✅
Candle                       ✅
CandleInterval               ✅
CandleResponse               ✅
IExchange                    ✅
Dependency Injection         ✅
BingXFuturesExchange         ✅
BingX Price                  ✅
BingX Candles                ✅
BingX DTOs                   ✅
BingX Candle Mapper          ✅
Interval Mapper              ✅
API Price Endpoint           ✅
API Candles Endpoint         ✅
BTC-USDT 1m                  ✅
BTC-USDT 5m                  ✅
Orden cronológico            ✅
```

---

# 🟡 Próximo Sprint

## API Hardening

Antes de continuar hacia la web, CryptoHub debe pasar por una etapa de limpieza y robustez.

Objetivos:

1. Validar `limit`.
2. Definir un límite máximo para `limit`.
3. Validar intervalos inválidos.
4. Mejorar el manejo de errores de BingX.
5. Eliminar código temporal de debugging.
6. Revisar la advertencia de `Microsoft.OpenApi`.
7. Probar casos válidos e inválidos.
8. Revisar los contratos públicos de los endpoints.
9. Documentar y realizar un commit estable.

Después de esto se continuará ampliando la API de market data y posteriormente se comenzará la construcción de la interfaz web.

---

# 🏁 Hito de la sesión

CryptoHub dejó de ser solamente una API capaz de obtener un precio.

Ahora CryptoHub puede:

```text
Consultar precio real
        +
Consultar velas reales
        +
Transformar datos externos
        +
Exponer un contrato propio
```

La API pública ya puede entregar información de mercado preparada para ser consumida posteriormente por:

- La web de CryptoHub.
- La futura aplicación para iPhone.
- Otros clientes.

---

# 💭 Reflexión

Hoy ocurrió algo importante en el proyecto.

CryptoHub ya está recibiendo información real de un exchange, pero el resto del sistema no necesita conocer cómo BingX representa sus datos.

BingX tiene su propio formato.

CryptoHub tiene su propio dominio.

Y el mapper funciona como frontera entre ambos.

```text
BingX
  ↓
DTO externo
  ↓
Mapper
  ↓
Dominio CryptoHub
  ↓
API CryptoHub
```

Esto refuerza una de las decisiones arquitectónicas más importantes del proyecto:

> CryptoHub no debe convertirse en un simple cliente de BingX.

Debe convertirse en una plataforma propia.

---

# ☕ Quote of the Day

> "No debemos diseñar nuestra aplicación alrededor del formato del proveedor. El proveedor debe adaptarse a nuestro dominio."

---
