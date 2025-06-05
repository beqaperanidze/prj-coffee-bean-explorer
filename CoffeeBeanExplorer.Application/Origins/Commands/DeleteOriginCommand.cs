using CoffeeBeanExplorer.Application.Common.Attributes;
using CoffeeBeanExplorer.Domain.Repositories;
using MediatR;

namespace CoffeeBeanExplorer.Application.Origins.Commands;

[TrackMetrics(MetricType.Delete)]
public record DeleteOriginCommand(int Id) : IRequest<bool>;

public class DeleteOriginCommandHandler(IOriginRepository repository) : IRequestHandler<DeleteOriginCommand, bool>
{
    public async Task<bool> Handle(DeleteOriginCommand request, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(request.Id);
    }
}
