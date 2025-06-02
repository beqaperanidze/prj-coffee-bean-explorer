using CoffeeBeanExplorer.Application.Common.Attributes;
using CoffeeBeanExplorer.Application.DTOs;
using MediatR;

namespace CoffeeBeanExplorer.Application.Origins.Queries;

[TrackMetrics("Update")]
public record UpdateOriginCommand(int Id, UpdateOriginDto Dto) : IRequest<bool>;
