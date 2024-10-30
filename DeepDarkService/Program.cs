using DeepDarkService.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCustomServices(builder.Configuration);
builder.Configuration.AddJsonFile("appsettings.json", optional: false);

var app = builder.Build();
app.UseCustomMiddleware();
app.Run();