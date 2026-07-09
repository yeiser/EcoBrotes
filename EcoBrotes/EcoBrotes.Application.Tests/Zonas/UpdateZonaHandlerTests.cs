using EcoBrotes.Application.Ports;
using EcoBrotes.Application.Zonas.Command;
using EcoBrotes.Domain.Exceptions;
using EcoBrotes.Domain.ZonaUrbana.Entity;
using NSubstitute;

namespace EcoBrotes.Application.Tests.Zonas
{
    public class UpdateZonaHandlerTests
    {
        private readonly IRepository<ZonaUrbanaEntity> _zonaRepository = Substitute.For<IRepository<ZonaUrbanaEntity>>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

        private UpdateZonaHandler BuildHandler() => new(_zonaRepository, _unitOfWork);

        [Fact]
        public async Task Handle_WithActiveZona_UpdatesNameAndSaves()
        {
            // Arrange
            var zona = new ZonaUrbanaEntity { Name = "Zona Norte" };
            var id = Guid.NewGuid();
            _zonaRepository.GetByIdAsync(id).Returns(zona);
            var command = new UpdateZonaCommand(id, "Zona Sur");

            // Act
            await BuildHandler().Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("Zona Sur", zona.Name);
            await _unitOfWork.Received(1).SaveAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WithMissingZona_ThrowsRequiredException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _zonaRepository.GetByIdAsync(id).Returns((ZonaUrbanaEntity)null!);
            var command = new UpdateZonaCommand(id, "Zona Sur");

            // Act & Assert
            await Assert.ThrowsAsync<RequiredException>(
                () => BuildHandler().Handle(command, CancellationToken.None));
            await _unitOfWork.DidNotReceive().SaveAsync(Arg.Any<CancellationToken>());
        }

        [Trait("ReglaNegocio", "RB-06")]
        [Fact]
        public async Task Handle_WithInactiveZona_ThrowsCoreBusinessException()
        {
            // Arrange
            var zona = new ZonaUrbanaEntity { Name = "Zona Norte" };
            zona.Deactivate();
            var id = Guid.NewGuid();
            _zonaRepository.GetByIdAsync(id).Returns(zona);
            var command = new UpdateZonaCommand(id, "Zona Sur");

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CoreBusinessException>(
                () => BuildHandler().Handle(command, CancellationToken.None));
            Assert.Contains("desactivada", exception.Message);
            await _unitOfWork.DidNotReceive().SaveAsync(Arg.Any<CancellationToken>());
        }
    }
}
