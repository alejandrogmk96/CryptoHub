using System.Text.Json;
using CryptoHub.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();
builder.Services.AddScoped<MarketService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.MapGet("/market/{symbol}", async (string symbol, MarketService marketService) =>
{
    try
{
    var ticker = await marketService.GetTickerAsync(symbol);

    return Results.Ok(ticker);
}
    catch (Exception ex)
{
    return Results.Problem(ex.Message);
}
});


app.Run();

