using System.Reflection;
using CoffeeBeanExplorer.Application.Common.Attributes;
using Microsoft.Extensions.Logging;

namespace CoffeeBeanExplorer.Application.Common.Services;

public class AttributeReaderService(ILogger<AttributeReaderService> logger)
{
    public static TrackMetricsAttribute? GetTrackMetricsAttribute<TRequest>(TRequest request)
    {
        var type = request!.GetType();
        return type.GetCustomAttribute<TrackMetricsAttribute>();
    }
}
