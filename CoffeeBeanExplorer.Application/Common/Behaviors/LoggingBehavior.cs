using System.Diagnostics;
using CoffeeBeanExplorer.Application.Common.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoffeeBeanExplorer.Application.Common.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        AttributeReaderService attributeReader)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            logger.LogInformation("Handling request: {RequestName}", requestName);

            var metricsAttribute = attributeReader.GetTrackMetricsAttribute(request);
            var logPerformance = metricsAttribute?.LogPerformance ?? false;

            var stopwatch = logPerformance ? Stopwatch.StartNew() : null;

            try
            {
                var response = await next(cancellationToken);

                if (logPerformance && stopwatch != null)
                {
                    stopwatch.Stop();
                    logger.LogInformation(
                        "Handled {RequestName} in {ElapsedMilliseconds}ms",
                        requestName, stopwatch.ElapsedMilliseconds);
                }

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error handling {RequestName}: {ErrorMessage}", requestName, ex.Message);
                throw;
            }
        }
    }
}
