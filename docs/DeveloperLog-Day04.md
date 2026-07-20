# CryptoHub - Developer Log

## Día 4 - Descubriendo el dominio antes del código

**Fecha:** 19 de julio de 2026

------------------------------------------------------------------------

# Objetivo del día

Continuar el diseño de la arquitectura de CryptoHub sin comenzar aún con
las implementaciones.

El objetivo principal fue entender qué información necesita el proyecto
antes de escribir código.

# Temas estudiados

-   Organización del proyecto.
-   Contratos (Interfaces).
-   Modelos del dominio.
-   Diseño antes de implementación.
-   Introducción a Records.
-   Objetos inmutables.
-   Responsabilidad de un modelo.

# Arquitectura del proyecto

``` text
CryptoHub.Api
│
├── Contracts
├── Models
├── Services
├── Properties
└── Program.cs
```

# Contratos

Se creó la carpeta **Contracts** y el archivo **IExchange.cs**.

Por decisión de diseño, la interfaz permanece vacía mientras se define
correctamente el dominio que utilizará.

``` csharp
namespace CryptoHub.Api.Contracts;

public interface IExchange
{

}
```

No es código incompleto; es una decisión consciente de arquitectura.

# Modelos

Se descubrió el primer modelo del dominio:

## TradingPair

Inicialmente se pensó en utilizar un `string` como:

``` text
BTC-USDT
```

Después del análisis se concluyó que CryptoHub no debe depender del
formato de ningún Exchange.

Cada proveedor puede representar el mismo par de forma distinta:

``` text
BTC-USDT
BTCUSDT
BTC_USDT
```

Por ello el proyecto utilizará un modelo propio.

``` csharp
namespace CryptoHub.Api.Models;

public record TradingPair(string BaseAsset, string QuoteAsset);
```

## Responsabilidad

Un `TradingPair` representa un único par de trading.

Ejemplos:

-   BTC / USDT
-   ETH / USDC
-   SOL / MXN

El objeto no debe cambiar de identidad después de crearse.

# Class vs Record

Se decidió utilizar un **record** porque representa información del
dominio y no un objeto con comportamiento complejo.

# Descubrimiento importante

Durante el diseño de `IExchange` surgió una nueva pregunta:

> ¿Qué debe devolver `GetPrice()`?

La conclusión fue que un simple `decimal` probablemente no será
suficiente.

Esto llevó al descubrimiento de un nuevo modelo del dominio:

-   `Price`

Su diseño quedó pendiente para la siguiente sesión.

# Estado actual

``` text
CryptoHub.Api
│
├── Contracts
│   └── IExchange.cs
│
├── Models
│   └── TradingPair.cs
│
├── Services
│
└── Program.cs
```

# Lección del día

> No diseñamos clases porque el lenguaje las permita.
>
> Diseñamos modelos porque el dominio los necesita.

# Conceptos aprendidos

-   Organización del proyecto.
-   Interfaces.
-   Records.
-   Objetos inmutables.
-   Modelado del dominio.
-   Diseño antes de implementación.

# Próxima sesión

-   Diseñar el modelo `Price`.
-   Completar `IExchange` con el método `GetPrice(...)`.

# Frase del día

> "La sintaxis se aprende rápido. Diseñar software es la habilidad que
> realmente hace crecer a un desarrollador."
