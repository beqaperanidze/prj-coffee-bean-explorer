using CoffeeBeanExplorer.Application.Common.Attributes;
using MediatR;

namespace CoffeeBeanExplorer.Application.Origins.Queries;

[TrackMetrics("Delete")]
public record DeleteOriginCommand(int Id) : IRequest<bool>;
