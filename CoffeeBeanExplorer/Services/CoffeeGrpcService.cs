using Grpc.Core;

namespace CoffeeBeanExplorer.Services;

public class CoffeeGrpcService(ILogger<CoffeeGrpcService> logger) : CoffeeService.CoffeeServiceBase
{
    private readonly ILogger<CoffeeGrpcService> _logger = logger;

    public override Task<CoffeeResponse> GetCoffee(CoffeeRequest request, ServerCallContext context)
    {
        var response = new CoffeeResponse
        {
            Message = $"Coffee info for {request.Name}"
        };

        return Task.FromResult(response);
    }
}
