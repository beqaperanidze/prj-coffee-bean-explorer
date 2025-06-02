using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using CoffeeBeanExplorer.Application;
using CoffeeBeanExplorer.Application.Common.Behaviors;
using CoffeeBeanExplorer.Application.Common.Services;
using CoffeeBeanExplorer.Application.Mapping;
using CoffeeBeanExplorer.Application.Services.Implementations;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using CoffeeBeanExplorer.Configuration;
using CoffeeBeanExplorer.Domain.Repositories;
using CoffeeBeanExplorer.Infrastructure.Data;
using CoffeeBeanExplorer.Infrastructure.Repositories;
using CoffeeBeanExplorer.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddValidatorsFromAssemblyContaining<ApplicationAssemblyMarker>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Coffee Bean Explorer API",
        Version = "v1"
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
if (!Directory.Exists(logDirectory)) Directory.CreateDirectory(logDirectory);

builder.Host.UseSerilog((context, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.Seq("http://localhost:5342", apiKey: null,
            restrictedToMinimumLevel: LogEventLevel.Information)
        .WriteTo.File(
            Path.Combine(AppContext.BaseDirectory, "logs", "app.log"),
            rollingInterval: RollingInterval.Day,
            outputTemplate:
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Properties:j}{NewLine}{Exception}")
        .Enrich.WithProperty("ApplicationName", "CoffeeBeanExplorer")
        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithThreadId()
        .Enrich.WithCorrelationId()
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("CoffeeBeanExplorer.Application.Common.Behaviors", LogEventLevel.Debug));
Log.Logger.Information("Application starting - testing log file creation");
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});
builder.Services.AddTransient<AttributeReaderService>();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
});

builder.Services.AddGrpc();
builder.Services.AddAutoMapper(
    typeof(UserProfile),
    typeof(BeanProfile),
    typeof(OriginProfile),
    typeof(ReviewProfile),
    typeof(TagProfile),
    typeof(UserListProfile)
);
builder.Services.Configure<RateLimitSettings>(
    builder.Configuration.GetSection("RateLimit"));

builder.Services.AddRateLimiter(options =>
{
    var rateLimitSettings = builder.Configuration.GetSection("RateLimit").Get<RateLimitSettings>();

    options.AddFixedWindowLimiter("global", config =>
    {
        config.PermitLimit = rateLimitSettings?.PermitLimit ?? 100;
        config.Window = TimeSpan.FromMinutes(rateLimitSettings?.WindowMinutes ?? 1);
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        config.QueueLimit = rateLimitSettings?.QueueLimit ?? 10;
    });
});
builder.Services.AddScoped<DbConnectionFactory>();

builder.Services.AddScoped<IBeanRepository, BeanRepository>();
builder.Services.AddScoped<IOriginRepository, OriginRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserListRepository, UserListRepository>();
builder.Services.AddScoped<IBeanService, BeanService>();
builder.Services.AddScoped<IOriginService, OriginService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserListService, UserListService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Coffee Bean Explorer API v1"); });
}

app.UseHttpsRedirection();

app.UseRateLimiter();
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        var correlationId = httpContext.TraceIdentifier;
        diagnosticContext.Set("CorrelationId", correlationId);
        diagnosticContext.Set("RequestHost", httpContext.Request.Host);
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent);
    };
});
app.UseAuthorization();
app.MapControllers();
app.MapGrpcService<CoffeeGrpcService>();

app.Run();
