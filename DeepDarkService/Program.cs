using DeepDarkService.Extensions;

// TODO: remove
Environment.SetEnvironmentVariable("Maps", @"C:\Users\vasili\Desktop\MIPT_4\DigitalMIPT\");
Environment.SetEnvironmentVariable("Threshold", "0.8");
Environment.SetEnvironmentVariable("SBERT_VOCAB", @"C:\Users\vasili\Desktop\MIPT_4\DigitalMIPT\sbert\MyFirstApp\bin\Debug\net8.0\tokenizer.json");
Environment.SetEnvironmentVariable("SBERT_MODEL", @"C:\Users\vasili\Desktop\MIPT_4\DigitalMIPT\sbert\MyFirstApp\bin\Debug\net8.0\sbert_onnx.onnx");


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCustomServices(builder.Configuration);
builder.Configuration.AddJsonFile("appsettings.json", optional: false);

var app = builder.Build();
app.UseCustomMiddleware();
app.Run();