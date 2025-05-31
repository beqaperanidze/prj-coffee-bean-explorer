using CoffeeBeanExplorer.Application.DTOs;
using MediatR;

namespace CoffeeBeanExplorer.Application.Origins.Queries;

public record GetAllOriginsQuery : IRequest<IEnumerable<OriginDto>>;