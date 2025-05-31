using CoffeeBeanExplorer.Application.Origins.Queries;
using CoffeeBeanExplorer.Domain.Repositories;
using MediatR;

namespace CoffeeBeanExplorer.Application.Origins.Handlers;

public class UpdateOriginCommandHandler(IOriginRepository repository) : IRequestHandler<UpdateOriginCommand, bool>
{
    public async Task<bool> Handle(UpdateOriginCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByIdAsync(request.Id);
        if (existing == null) return false;

        existing.Country = request.Dto.Country;
        existing.Region = request.Dto.Region;

        return await repository.UpdateAsync(existing);
    }
}