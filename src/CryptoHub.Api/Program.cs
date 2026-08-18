using CryptoHub.Api.Contracts;
using CryptoHub.Api.Models;
using CryptoHub.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

builder.Services.AddOpenApi();
builder.Services.AddScoped<IExchange, BingXFuturesExchange>();
builder.Services.AddScoped<TradingPairParser>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

app.MapGet("/market/{symbol}", async (
    string symbol,
    IExchange exchange,
    TradingPairParser parser) =>
{
    try
    {
        var tradingPair = parser.Parse(symbol);

        var price = await exchange.GetPriceAsync(tradingPair);

        return Results.Ok(price);
    }
   catch (ArgumentException)
{
    return Results.BadRequest(
        new ApiError(
            "invalid_trading_pair",
            "El TradingPair debe tener el formato BASE-QUOTE."));
}
catch (Exception ex)
{
    return Results.Problem(ex.Message);
}
    
});

app.Run();