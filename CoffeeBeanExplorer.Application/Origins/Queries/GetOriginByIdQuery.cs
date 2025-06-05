using AutoMapper;
using CoffeeBeanExplorer.Application.Common.Attributes;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Domain.Repositories;
using MediatR;

namespace CoffeeBeanExplorer.Application.Origins.Queries;

[TrackMetrics(MetricType.Query)]
public record GetOriginByIdQuery(int Id) : IRequest<OriginDto?>;

public class GetOriginByIdQueryHandler(IOriginRepository repository, IMapper mapper)
    : IRequestHandler<GetOriginByIdQuery, OriginDto?>
{
    public async Task<OriginDto?> Handle(GetOriginByIdQuery request, CancellationToken cancellationToken)
    {
        var origin = await repository.GetByIdAsync(request.Id);
        return origin == null ? null : mapper.Map<OriginDto>(origin);
    }
}