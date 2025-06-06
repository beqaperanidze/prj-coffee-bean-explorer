using System.Diagnostics;
using CoffeeBeanExplorer.Application.Common.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace CoffeeBeanExplorer.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger,
    IHttpContextAccessor httpContextAccessor)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        var httpContext = httpContextAccessor.HttpContext;
        var correlationId = httpContext?.TraceIdentifier ?? Guid.NewGuid().ToString();
        var path = httpContext?.Request.Path.Value ?? "Unknown";

        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("RequestType", requestName))
        using (LogContext.PushProperty("Path", path))
        {
            logger.LogInformation("[{CorrelationId}] Handling {RequestType} for path {Path}",
                correlationId, requestName, path);

            var metricsAttribute = AttributeReaderService.GetTrackMetricsAttribute(request);
            var logPerformance = metricsAttribute?.LogPerformance ?? false;
            var stopwatch = logPerformance ? Stopwatch.StartNew() : null;

            try
            {
                var response = await next(cancellationToken);

                if (stopwatch is not null)
                {
                    stopwatch.Stop();
                    var elapsed = stopwatch.ElapsedMilliseconds;

                    using (LogContext.PushProperty("ElapsedMs", elapsed))
                    {
                        logger.LogInformation(
                            "[{CorrelationId}] Handled {RequestType} in {ElapsedMilliseconds}ms",
                            correlationId, requestName, elapsed);
                    }
                }
                else
                {
                    logger.LogInformation("[{CorrelationId}] Handled {RequestType} successfully",
                        correlationId, requestName);
                }

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "[{CorrelationId}] Error handling {RequestType}: {ErrorType} - {ErrorMessage}",
                    correlationId, requestName, ex.GetType().Name, ex.Message);
                throw;
            }
        }
    }
}