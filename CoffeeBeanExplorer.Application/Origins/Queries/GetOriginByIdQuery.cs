using CoffeeBeanExplorer.Application.DTOs;
using MediatR;

namespace CoffeeBeanExplorer.Application.Origins.Queries;

public record GetOriginByIdQuery(int Id) : IRequest<OriginDto?>;