using MediatR;

namespace CoffeeBeanExplorer.Application.Origins.Queries;

public record DeleteOriginCommand(int Id) : IRequest<bool>;