namespace CoffeeBeanExplorer.Application.Common.Attributes;

public class TrackMetricsAttribute(string operationType) : Attribute
{
    public string OperationType { get; } = operationType;
    public bool LogPerformance { get; set; } = true;
}
