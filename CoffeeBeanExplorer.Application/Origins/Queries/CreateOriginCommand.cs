using CoffeeBeanExplorer.Application.DTOs;
using MediatR;

namespace CoffeeBeanExplorer.Application.Origins.Queries;

public record CreateOriginCommand(CreateOriginDto Dto) : IRequest<OriginDto>;