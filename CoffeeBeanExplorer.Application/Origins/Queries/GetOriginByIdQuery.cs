using CoffeeBeanExplorer.Application.Common.Attributes;
using CoffeeBeanExplorer.Application.DTOs;
using MediatR;

namespace CoffeeBeanExplorer.Application.Origins.Queries;

[TrackMetrics("Query")]
public record GetOriginByIdQuery(int Id) : IRequest<OriginDto?>;
