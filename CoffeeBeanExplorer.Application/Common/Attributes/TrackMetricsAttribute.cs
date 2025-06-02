namespace CoffeeBeanExplorer.Application.Common.Attributes;

/// <summary>
///     Attribute to indicate that a request or handler should have metrics tracking enabled.
///     The <see cref="Metric" /> property describes the type of operation being tracked (e.g., MetricType.Create).
///     The <see cref="LogPerformance" /> property controls whether performance (timing) is logged.
///     Intended for use on MediatR request classes or handlers.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class TrackMetricsAttribute(MetricType metric) : Attribute
{
    public MetricType Metric { get; } = metric;
    public bool LogPerformance { get; set; } = true;
}