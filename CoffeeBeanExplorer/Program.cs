using System.Reflection;
using CoffeeBeanExplorer.Configuration;
using CoffeeBeanExplorer.Services;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Coffee Bean Explorer API",
        Version = "v1"
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

builder.Services.AddGrpc();

builder.Services.Configure<RateLimitSettings>(
    builder.Configuration.GetSection("RateLimit"));

builder.Services.AddRateLimiter(options =>
{
    var rateLimitSettings = builder.Configuration.GetSection("RateLimit").Get<RateLimitSettings>();

    options.AddFixedWindowLimiter("global", config =>
    {
        config.PermitLimit = rateLimitSettings?.PermitLimit ?? 100;
        config.Window = TimeSpan.FromMinutes(rateLimitSettings?.WindowMinutes ?? 1);
        config.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        config.QueueLimit = rateLimitSettings?.QueueLimit ?? 10;
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Coffee Bean Explorer API v1"); });
}

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAuthorization();
app.MapControllers();
app.MapGrpcService<CoffeeGrpcService>();

app.Run();
