using CoffeeBeanExplorer.Application.DTOs;
using MediatR;

namespace CoffeeBeanExplorer.Application.Origins.Queries;

public record UpdateOriginCommand(int Id, UpdateOriginDto Dto) : IRequest<bool>;