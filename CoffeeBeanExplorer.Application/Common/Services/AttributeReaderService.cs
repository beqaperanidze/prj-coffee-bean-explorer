using System.Reflection;
using CoffeeBeanExplorer.Application.Common.Attributes;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoffeeBeanExplorer.Application.Common.Services
{
    public class AttributeReaderService(ILogger<AttributeReaderService> logger)
    {
        public TrackMetricsAttribute? GetTrackMetricsAttribute<TRequest>(TRequest request)
        {
            var type = request.GetType();
            var attribute = type.GetCustomAttribute<TrackMetricsAttribute>();

            if (attribute != null)
            {
                logger.LogInformation(
                    "Request {RequestType} has metrics tracking with operation type: {OperationType}",
                    type.Name, attribute.OperationType);
            }

            return attribute;
        }
    }
}
