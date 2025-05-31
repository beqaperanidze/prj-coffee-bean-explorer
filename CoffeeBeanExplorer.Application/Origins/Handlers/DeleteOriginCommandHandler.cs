using CoffeeBeanExplorer.Application.Origins.Queries;
using CoffeeBeanExplorer.Domain.Repositories;
using MediatR;

namespace CoffeeBeanExplorer.Application.Origins.Handlers;

public class DeleteOriginCommandHandler(IOriginRepository repository) : IRequestHandler<DeleteOriginCommand, bool>
{
    public async Task<bool> Handle(DeleteOriginCommand request, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(request.Id);
    }
}