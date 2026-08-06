# Developer Log - Day 06 (Price Domain Model)

## Objetivo

Hoy diseñamos nuestro primer modelo de dominio para CryptoHub: `Price`.

El objetivo no fue únicamente escribir una clase en C#, sino comprender
qué representa un precio dentro del dominio y cómo proteger las reglas
del negocio desde el constructor.

## Temas aprendidos

### ¿Qué representa `Price`?

Un `Price` representa:

-   El precio de un `TradingPair`.
-   En un instante específico.
-   Como un hecho histórico.

Por ello, una vez creado, sus propiedades no deben cambiar.

### Propiedades

``` csharp
public TradingPair TradingPair { get; }
public decimal Value { get; }
public DateTimeOffset Timestamp { get; }
```

Aprendizajes:

-   `TradingPair` identifica el activo.
-   `decimal` se utiliza por la precisión financiera.
-   `DateTimeOffset` representa correctamente el instante en distintas
    zonas horarias.

### Constructor

``` csharp
public Price(
    TradingPair tradingPair,
    decimal value,
    DateTimeOffset timestamp)
```

Comprendimos que el constructor garantiza que un objeto nazca completo y
válido.

### Validaciones

#### TradingPair obligatorio

``` csharp
ArgumentNullException.ThrowIfNull(tradingPair);
```

Aprendizaje:

-   `ThrowIfNull()` detiene la construcción cuando un argumento
    obligatorio es `null`.

#### Precio válido

``` csharp
if (value < 0)
{
    throw new ArgumentOutOfRangeException(
        nameof(value),
        "El precio no puede ser negativo.");
}
```

Aprendizajes:

-   `throw` detiene inmediatamente la ejecución.
-   `ArgumentOutOfRangeException` comunica que un argumento está fuera
    del rango permitido.
-   `nameof(value)` obtiene automáticamente el nombre del parámetro,
    evitando errores durante futuras refactorizaciones.

### Excepciones

Durante la sesión comprendimos la diferencia entre:

-   `Exception`
-   `ArgumentException`
-   `ArgumentOutOfRangeException`
-   `InvalidOperationException`

El criterio aprendido fue elegir siempre la excepción que mejor
comunique el problema.

## Clase final

``` csharp
namespace CryptoHub.Api.Models;

public class Price
{
    public TradingPair TradingPair { get; }

    public decimal Value { get; }

    public DateTimeOffset Timestamp { get; }

    public Price(
        TradingPair tradingPair,
        decimal value,
        DateTimeOffset timestamp)
    {
        ArgumentNullException.ThrowIfNull(tradingPair);

        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                "El precio no puede ser negativo.");
        }

        TradingPair = tradingPair;
        Value = value;
        Timestamp = timestamp;
    }
}
```

## Conceptos nuevos

-   Constructor
-   `throw`
-   `ArgumentNullException.ThrowIfNull()`
-   `ArgumentOutOfRangeException`
-   `nameof()`
-   `decimal`
-   `DateTimeOffset`

## Estado final

-   Modelo `TradingPair` terminado.
-   Modelo `Price` terminado y validado.
-   Primeras reglas del dominio implementadas mediante excepciones.

## Próxima sesión

-   Interfaz `IExchange`.
-   Implementación de `BingXExchange`.
-   Primer consumo de una API HTTP desde CryptoHub.
