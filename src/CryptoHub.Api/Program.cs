using CryptoHub.Api.Contracts;
using CryptoHub.Api.Endpoints;
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

app.MapMarketEndpoints();

app.Run();