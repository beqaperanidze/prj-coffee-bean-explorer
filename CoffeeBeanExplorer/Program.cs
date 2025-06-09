using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using CoffeeBeanExplorer.Application;
using CoffeeBeanExplorer.Application.Common.Behaviors;
using CoffeeBeanExplorer.Application.Common.Services;
using CoffeeBeanExplorer.Application.Services.Implementations;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using CoffeeBeanExplorer.Configuration;
using CoffeeBeanExplorer.Domain.Configuration;
using CoffeeBeanExplorer.Domain.Exceptions;
using CoffeeBeanExplorer.Domain.Repositories;
using CoffeeBeanExplorer.Infrastructure.Data;
using CoffeeBeanExplorer.Infrastructure.Repositories;
using CoffeeBeanExplorer.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

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

if (builder.Configuration.GetValue("Swagger:Enabled", true))
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc(
            builder.Configuration["Swagger:Version"] ?? "v1",
            new OpenApiInfo
            {
                Title = builder.Configuration["Swagger:Title"] ?? "Coffee Bean Explorer API",
                Version = builder.Configuration["Swagger:Version"] ?? "v1"
            });

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description =
                "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                []
            }
        });

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);
    });

var logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
if (!Directory.Exists(logDirectory)) Directory.CreateDirectory(logDirectory);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));


var apiVersionSection = builder.Configuration.GetSection("ApiVersioning");
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified =
        apiVersionSection.GetValue<bool>("AssumeDefaultVersionWhenUnspecified");

    var versionStr = apiVersionSection["DefaultApiVersion"] ?? "1.0";
    var versionParts = versionStr.Split('.');
    if (versionParts.Length == 2 &&
        int.TryParse(versionParts[0], out var major) &&
        int.TryParse(versionParts[1], out var minor))
        options.DefaultApiVersion = new ApiVersion(major, minor);
    else
        options.DefaultApiVersion = new ApiVersion(1, 0);

    options.ReportApiVersions = apiVersionSection.GetValue<bool>("ReportApiVersions");
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
builder.Services.AddAutoMapper(typeof(ApplicationAssemblyMarker).Assembly);

builder.Services.Configure<RateLimitSettings>(
    builder.Configuration.GetSection("RateLimit"));

builder.Services.AddRateLimiter(options =>
{
    var rateLimitSection = builder.Configuration.GetSection("RateLimit");

    options.AddFixedWindowLimiter("global", config =>
    {
        config.PermitLimit = rateLimitSection.GetValue("PermitLimit", 100);
        config.Window = TimeSpan.FromMinutes(rateLimitSection.GetValue("WindowMinutes", 1));
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        config.QueueLimit = rateLimitSection.GetValue("QueueLimit", 10);
    });
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ??
                                       throw new InvalidOperationException("JWT key is not configured"))),
            ClockSkew = TimeSpan.Zero
        };
    });


builder.Services.AddAuthorization();

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
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddHealthChecks()
    .AddCheck("API", () => HealthCheckResult.Healthy("API is responding"), ["api"])
    .AddNpgSql(
        builder.Configuration.GetConnectionString("DefaultConnection") ?? "",
        name: "database",
        tags: ["db", "postgres", "postgresql"],
        timeout: TimeSpan.FromSeconds(5));

var app = builder.Build();

if (app.Environment.IsDevelopment() && builder.Configuration.GetValue("Swagger:Enabled", true))
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint(
            $"/swagger/{builder.Configuration["Swagger:Version"] ?? "v1"}/swagger.json",
            $"{builder.Configuration["Swagger:Title"] ?? "Coffee Bean Explorer API"} {builder.Configuration["Swagger:Version"] ?? "v1"}");
    });
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

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGrpcService<CoffeeGrpcService>();
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/detail", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            duration = report.TotalDuration,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration
            })
        });

        await context.Response.WriteAsync(result);
    }
});

app.Run();
