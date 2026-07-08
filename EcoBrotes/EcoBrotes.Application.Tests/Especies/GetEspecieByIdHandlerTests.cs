using EcoBrotes.Application.Especies.Query;
using EcoBrotes.Domain.EspecieArborea.Entity;
using EcoBrotes.Domain.EspecieArborea.Port;
using EcoBrotes.Domain.Exceptions;
using NSubstitute;

namespace EcoBrotes.Application.Tests.Especies
{
    public class GetEspecieByIdHandlerTests
    {
        private readonly IEspecieArboreaRepository _especieRepository = Substitute.For<IEspecieArboreaRepository>();

        private GetEspecieByIdHandler BuildHandler() => new(_especieRepository);

        [Fact]
        public async Task Handle_WithExistingEspecie_ReturnsMappedDto()
        {
            // Arrange
            var id = Guid.NewGuid();
            var especie = new EspecieArboreaEntity { Id = id, Name = "Roble", ScientificName = "Quercus", MaxHeightMeters = 20m };
            _especieRepository.GetByIdAsync(id).Returns(especie);
            var query = new GetEspecieByIdQuery { Id = id };

            // Act
            var dto = await BuildHandler().Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal("Roble", dto.Name);
            Assert.Equal("Quercus", dto.ScientificName);
            Assert.Equal(20m, dto.MaxHeightMeters);
        }

        [Fact]
        public async Task Handle_WithMissingEspecie_ThrowsCoreBusinessException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _especieRepository.GetByIdAsync(id).Returns((EspecieArboreaEntity)null!);
            var query = new GetEspecieByIdQuery { Id = id };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CoreBusinessException>(
                () => BuildHandler().Handle(query, CancellationToken.None));
            Assert.Contains("no existe", exception.Message);
        }
    }
}
