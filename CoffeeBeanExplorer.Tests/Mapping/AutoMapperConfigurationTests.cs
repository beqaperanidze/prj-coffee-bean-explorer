using AutoMapper;
using CoffeeBeanExplorer.Application;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CoffeeBeanExplorer.Tests.Mapping;

public class AutoMapperConfigurationTests
{
    [Fact]
    public void AutoMapper_Configuration_IsValid()
    {
        var services = new ServiceCollection();
        services.AddAutoMapper(typeof(ApplicationAssemblyMarker).Assembly);
        var serviceProvider = services.BuildServiceProvider();

        var mapper = serviceProvider.GetRequiredService<IMapper>();

        mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
}