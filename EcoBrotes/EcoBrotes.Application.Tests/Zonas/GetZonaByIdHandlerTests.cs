using EcoBrotes.Application.Zonas.Query;
using EcoBrotes.Domain.Exceptions;
using EcoBrotes.Domain.ZonaUrbana.Entity;
using EcoBrotes.Domain.ZonaUrbana.Port;
using NSubstitute;

namespace EcoBrotes.Application.Tests.Zonas
{
    public class GetZonaByIdHandlerTests
    {
        private readonly IZonaUrbanaRepository _zonaRepository = Substitute.For<IZonaUrbanaRepository>();

        private GetZonaByIdHandler BuildHandler() => new(_zonaRepository);

        [Fact]
        public async Task Handle_WithExistingZona_ReturnsMappedDto()
        {
            // Arrange
            var id = Guid.NewGuid();
            var zona = new ZonaUrbanaEntity { Id = id, Name = "Zona Norte" };
            _zonaRepository.GetByIdAsync(id).Returns(zona);
            var query = new GetZonaByIdQuery { Id = id };

            // Act
            var dto = await BuildHandler().Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal("Zona Norte", dto.Name);
        }

        [Fact]
        public async Task Handle_WithMissingZona_ThrowsCoreBusinessException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _zonaRepository.GetByIdAsync(id).Returns((ZonaUrbanaEntity)null!);
            var query = new GetZonaByIdQuery { Id = id };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CoreBusinessException>(
                () => BuildHandler().Handle(query, CancellationToken.None));
            Assert.Contains("no existe", exception.Message);
        }
    }
}
