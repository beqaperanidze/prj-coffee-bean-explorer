using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Application.Origins.Queries;
using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;
using MediatR;

namespace CoffeeBeanExplorer.Application.Origins.Handlers;

public class CreateOriginCommandHandler(IOriginRepository repository) : IRequestHandler<CreateOriginCommand, OriginDto>
{
    public async Task<OriginDto> Handle(CreateOriginCommand request, CancellationToken cancellationToken)
    {
        var origin = new Origin
        {
            Country = request.Dto.Country,
            Region = request.Dto.Region
        };

        var addedOrigin = await repository.AddAsync(origin);

        return new OriginDto
        {
            Id = addedOrigin.Id,
            Country = addedOrigin.Country,
            Region = addedOrigin.Region
        };
    }
}