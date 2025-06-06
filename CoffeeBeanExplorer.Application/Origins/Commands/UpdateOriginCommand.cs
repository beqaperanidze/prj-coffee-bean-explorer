using AutoMapper;
using CoffeeBeanExplorer.Application.Common.Attributes;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Domain.Repositories;
using MediatR;

namespace CoffeeBeanExplorer.Application.Origins.Commands;

[TrackMetrics(MetricType.Update)]
public record UpdateOriginCommand(int Id, UpdateOriginDto Dto) : IRequest<bool>;

public class UpdateOriginCommandHandler(IOriginRepository repository, IMapper mapper)
    : IRequestHandler<UpdateOriginCommand, bool>
{
    public async Task<bool> Handle(UpdateOriginCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByIdAsync(request.Id);
        if (existing == null) return false;

        mapper.Map(request.Dto, existing);

        return await repository.UpdateAsync(existing);
    }
}