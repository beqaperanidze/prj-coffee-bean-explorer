using AutoMapper;
using CoffeeBeanExplorer.Application.Common.Attributes;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Domain.Models;
using CoffeeBeanExplorer.Domain.Repositories;
using MediatR;

namespace CoffeeBeanExplorer.Application.Origins.Commands;

[TrackMetrics(MetricType.Create)]
public record CreateOriginCommand(CreateOriginDto Dto) : IRequest<OriginDto>;

public class CreateOriginCommandHandler(IOriginRepository repository, IMapper mapper)
    : IRequestHandler<CreateOriginCommand, OriginDto>
{
    public async Task<OriginDto> Handle(CreateOriginCommand request, CancellationToken cancellationToken)
    {
        var origin = mapper.Map<Origin>(request.Dto);
        var addedOrigin = await repository.AddAsync(origin);
        return mapper.Map<OriginDto>(addedOrigin);
    }
}
