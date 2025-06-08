using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
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

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        logger.LogError(exception, "An error occurred: {Message}", exception.Message);

        var statusCode = HttpStatusCode.InternalServerError;
        var errorCode = "InternalServerError";
        var message = "An unexpected error occurred.";

        if (exception is ApiException apiException)
        {
            statusCode = (HttpStatusCode)apiException.StatusCode;
            errorCode = apiException.ErrorCode;
            message = apiException.Message;
        }

        var response = new
        {
            errorCode,
            message
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }
}