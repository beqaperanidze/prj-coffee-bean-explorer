namespace CoffeeBeanExplorer.Application.Common.Attributes;

/// <summary>
///     Attribute to indicate that a request or handler should have metrics tracking enabled.
/// </summary>
/// <remarks>
///     Intended for use on MediatR request classes or handlers.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class TrackMetricsAttribute(MetricType metric) : Attribute
{
    /// <summary>
    ///     Gets the type of operation being tracked (e.g., MetricType.Create).
    /// </summary>
    public MetricType Metric { get; } = metric;

    /// <summary>
    ///     Gets or sets whether performance (timing) should be logged.
    ///     Defaults to true.
    /// </summary>
    public bool LogPerformance { get; set; } = true;
}