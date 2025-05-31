using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Origins.Queries;
using CoffeeBeanExplorer.Domain.Repositories;
using MediatR;

namespace CoffeeBeanExplorer.Application.Origins.Handlers;

public class GetOriginByIdQueryHandler(IOriginRepository repository) : IRequestHandler<GetOriginByIdQuery, OriginDto?>
{
    public async Task<OriginDto?> Handle(GetOriginByIdQuery request, CancellationToken cancellationToken)
    {
        var origin = await repository.GetByIdAsync(request.Id);
        if (origin == null) return null;

        return new OriginDto
        {
            Id = origin.Id,
            Country = origin.Country,
            Region = origin.Region
        };
    }
}