using CoffeeBeanExplorer.Application.Common.Attributes;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Domain.Repositories;
using MediatR;

namespace CoffeeBeanExplorer.Application.Origins.Queries;

[TrackMetrics(MetricType.Query)]
public record GetAllOriginsQuery : IRequest<IEnumerable<OriginDto>>;

public class GetAllOriginsQueryHandler(IOriginRepository repository)
    : IRequestHandler<GetAllOriginsQuery, IEnumerable<OriginDto>>
{
    public async Task<IEnumerable<OriginDto>> Handle(GetAllOriginsQuery request, CancellationToken cancellationToken)
    {
        var origins = await repository.GetAllAsync();
        return origins.Select(o => new OriginDto { Id = o.Id, Country = o.Country, Region = o.Region });
    }
}