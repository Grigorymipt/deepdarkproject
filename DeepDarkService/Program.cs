using DeepDarkService.Extensions;

// TODO: remove
Environment.SetEnvironmentVariable("Maps", "/home/pommes/app/Resources/Maps/");
Environment.SetEnvironmentVariable("Resources", "/home/pommes/app/Resources/");
Environment.SetEnvironmentVariable("Threshold", "0.8");
Environment.SetEnvironmentVariable("SBERT_VOCAB", "/home/pommes/app/Resources/tokenizer.json");
Environment.SetEnvironmentVariable("SBERT_MODEL", "/home/pommes/app/Resources/sbert_onnx_2.onnx");


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCustomServices(builder.Configuration);
builder.Configuration.AddJsonFile("appsettings.json", optional: false);

var app = builder.Build();
app.UseCustomMiddleware();
app.Run();