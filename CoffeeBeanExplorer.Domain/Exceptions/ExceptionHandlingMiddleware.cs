using System.Net;
using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CoffeeBeanExplorer.Domain.Exceptions;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var errorCode = "InternalServerError";
        var message = "An unexpected error occurred.";

        if (exception is ApiException apiException)
        {
            statusCode = (HttpStatusCode)apiException.StatusCode;
            errorCode = apiException.ErrorCode;
            message = apiException.Message;
        }

        object response;
        if (context.RequestServices.GetService<IHostEnvironment>()?.IsDevelopment() == true)
            response = new
            {
                errorCode,
                message,
                detail = exception.ToString(),
                stackTrace = exception.StackTrace,
                innerException = exception.InnerException?.Message
            };
        else
            response = new
            {
                errorCode,
                message
            };

        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }
}
