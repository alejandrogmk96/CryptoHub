using CryptoHub.Api.Contracts;
using CryptoHub.Api.Models;
using CryptoHub.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

builder.Services.AddOpenApi();
builder.Services.AddScoped<IExchange, BingXFuturesExchange>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

app.MapGet("/market/{symbol}", async (string symbol, IExchange exchange) =>
{
    try
    {
        var parts = symbol.Split('-');

        if (parts.Length != 2)
        {
            return Results.BadRequest(
                "El símbolo debe tener el formato BTC-USDT.");
        }

        var tradingPair = new TradingPair(
            parts[0],
            parts[1]);

        var price = await exchange.GetPriceAsync(tradingPair);

        return Results.Ok(price);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.Run();