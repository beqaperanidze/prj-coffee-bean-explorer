using CoffeeBeanExplorer.Application.Common.Attributes;
using CoffeeBeanExplorer.Application.DTOs;
using MediatR;

namespace CoffeeBeanExplorer.Application.Origins.Queries;

[TrackMetrics("Create")]
public record CreateOriginCommand(CreateOriginDto Dto) : IRequest<OriginDto>;
