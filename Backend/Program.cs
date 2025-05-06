using Backend.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
var app = builder.Build();
app.UseDefaultFiles();
var tickers = Backend.TickerDatas.Tickers;

app.MapGet("/", () => "Ticker Backend Application");

app.MapHub<TickerHubs>("/Ticker");
app.Run();
