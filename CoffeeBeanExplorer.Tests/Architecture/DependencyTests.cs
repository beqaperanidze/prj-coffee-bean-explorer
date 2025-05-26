using NetArchTest.Rules;
using System.Reflection;
using Xunit;

namespace CoffeeBeanExplorer.Tests.Architecture
{
    public class DependencyTests
    {
        private readonly Assembly _domainAssembly = Assembly.Load("CoffeeBeanExplorer.Domain");
        private readonly Assembly _applicationAssembly = Assembly.Load("CoffeeBeanExplorer.Application");

        [Fact]
        public void Domain_Should_Not_HaveDependencyOnOtherProjects()
        {
            var otherProjects = new[]
            {
                "CoffeeBeanExplorer.Application",
                "CoffeeBeanExplorer.Infrastructure"
            };

            var result = Types
                .InAssembly(_domainAssembly)
                .ShouldNot()
                .HaveDependencyOnAny(otherProjects)
                .GetResult();

            var failingTypesMessage = result.FailingTypeNames != null
                ? string.Join(Environment.NewLine, result.FailingTypeNames)
                : "No failing types found, but test still failed";

            Assert.True(result.IsSuccessful, failingTypesMessage);
        }

        [Fact]
        public void Application_Should_OnlyHaveDependencyOnDomain()
        {
            var forbiddenProjects = new[]
            {
                "CoffeeBeanExplorer.Infrastructure"
            };

            var result = Types
                .InAssembly(_applicationAssembly)
                .ShouldNot()
                .HaveDependencyOnAny(forbiddenProjects)
                .GetResult();

            var failingTypesMessage = result.FailingTypeNames != null
                ? string.Join(Environment.NewLine, result.FailingTypeNames)
                : "No failing types found, but test still failed";

            Assert.True(result.IsSuccessful, failingTypesMessage);
        }
    }
}