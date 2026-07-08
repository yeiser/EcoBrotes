using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;
using EcoBrotes.Domain.Common;
using EcoBrotes.Infrastructure.Adapters;

namespace ArchitectureTests
{
    public class PruebasDeArquitectura
    {

        private static readonly Architecture Architecture = new ArchLoader().LoadAssemblies(
            System.Reflection.Assembly.Load("EcoBrotes.Api"),
            System.Reflection.Assembly.Load("EcoBrotes.Domain"),
            System.Reflection.Assembly.Load("EcoBrotes.Infrastructure"),
            System.Reflection.Assembly.Load("EcoBrotes.Application")
        ).Build();

        private readonly IObjectProvider<IType> DomainLayer = Classes()
            .That()
            .ResideInNamespace("EcoBrotes.Domain", true)
            .As("Domain Layer");

        private readonly IObjectProvider<IType> ApplicationLayer = Classes()
            .That()
            .ResideInNamespace("EcoBrotes.Application", true)
            .As("Application Layer");

        private readonly IObjectProvider<IType> InfrastructureLayer = Classes()
            .That()
            .ResideInNamespace("EcoBrotes.Infrastructure", true)
            .As("Infrastructure Layer");

        private readonly IObjectProvider<IType> ApiLayer = Classes()
            .That()
            .ResideInNamespace("EcoBrotes.Api", true)
            .As("API Layer");

        [Fact]
        public void DomainLayer_ShouldNotDependOnApplicationLayer()
        {
            Types()
                .That()
                .Are(DomainLayer)
                .Should()
                .NotDependOnAny(ApplicationLayer)
                .Because("Las capas internas no deben depender de capas de aplicacion")
                .Check(Architecture);
        }

        [Fact]
        public void DomainLayer_ShouldNotDependOnInfrastructureLayer()
        {
            Types()
            .That()
            .Are(DomainLayer)
            .Should()
            .NotDependOnAny(InfrastructureLayer)
            .Because("Las capas internas no deben depender de capas de infraestructura")
            .Check(Architecture);
        }

        [Fact]
        public void DomainLayer_ShouldNotDependOnApiLayer()
        {
            Types()
                .That()
                .Are(DomainLayer)
                .Should()
                .NotDependOnAny(ApiLayer)
                .Because("Las capas internas no deben depender de la capa Api")
                .Check(Architecture);
        }

        [Fact]
        public void DomainLayer_ShouldNotDependOnExternalLibraries()
        {
            var rule = Classes()
                .That().ResideInNamespace("EcoBrotes.Domain", true)
                .Should().NotDependOnAny("System.Net.Http")
                .AndShould().NotDependOnAny("Microsoft.EntityFrameworkCore");

            rule.Check(Architecture);
        }

        [Fact]
        public void Exceptions_ShouldInheritFromSystemException()
        {
            var rule = Classes()
                .That().ResideInNamespace("EcoBrotes.Domain.Exceptions", true)
                .Should().BeAssignableTo(typeof(System.Exception));

            rule.Check(Architecture);
        }
    }
}