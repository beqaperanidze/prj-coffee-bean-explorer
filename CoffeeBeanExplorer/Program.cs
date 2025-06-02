using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using CoffeeBeanExplorer.Application.Common.Behaviors;
using CoffeeBeanExplorer.Application.Common.Services;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Mapping;
using CoffeeBeanExplorer.Application.Origins.Queries;
using CoffeeBeanExplorer.Application.Services.Implementations;
using CoffeeBeanExplorer.Application.Services.Interfaces;
using CoffeeBeanExplorer.Application.Validators;
using CoffeeBeanExplorer.Configuration;
using CoffeeBeanExplorer.Domain.Repositories;
using CoffeeBeanExplorer.Infrastructure.Data;
using CoffeeBeanExplorer.Infrastructure.Repositories;
using CoffeeBeanExplorer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateOriginDtoValidator>();
builder.Services.AddScoped<IValidator<CreateOriginDto>, CreateOriginDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateOriginDto>, UpdateOriginDtoValidator>();
builder.Services.AddScoped<IValidator<CreateBeanDto>, CreateBeanDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateBeanDto>, UpdateBeanDtoValidator>();
builder.Services.AddScoped<IValidator<CreateBeanTagDto>, CreateBeanTagDtoValidator>();
builder.Services.AddScoped<IValidator<CreateListItemDto>, CreateListItemDtoValidator>();
builder.Services.AddScoped<IValidator<CreateReviewDto>, CreateReviewDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateReviewDto>, UpdateReviewDtoValidator>();
builder.Services.AddScoped<IValidator<CreateTagDto>, CreateTagDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateTagDto>, UpdateTagDtoValidator>();
builder.Services.AddScoped<IValidator<UserRegistrationDto>, UserRegistrationDtoValidator>();
builder.Services.AddScoped<IValidator<UserUpdateDto>, UserUpdateDtoValidator>();
builder.Services.AddScoped<IValidator<CreateUserListDto>, CreateUserListDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateUserListDto>, UpdateUserListDtoValidator>();

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
if (!Directory.Exists(logDirectory))
{
    Directory.CreateDirectory(logDirectory);
}

builder.Host.UseSerilog((context, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.Console()
        .WriteTo.File("D:/RiderProjects/CoffeeBeanExplorer/CoffeeBeanExplorer/bin/Debug/net9.0/logs/app.log",
            rollingInterval: RollingInterval.Day)
        .MinimumLevel
        .Override("CoffeeBeanExplorer.Application.Common.Behaviors",
            Serilog.Events.LogEventLevel.Debug));

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
    cfg.RegisterServicesFromAssembly(typeof(LoggingBehavior<,>).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateOriginCommand).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddGrpc();
builder.Services.AddAutoMapper(typeof(MappingProfile));
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
app.Use(async (context, next) =>
{
    Log.Information("Request received: {Path}", context.Request.Path);
    await next();
    Log.Information("Request completed: {Path}", context.Request.Path);
});
app.UseAuthorization();
app.MapControllers();
app.MapGrpcService<CoffeeGrpcService>();

app.Run();
