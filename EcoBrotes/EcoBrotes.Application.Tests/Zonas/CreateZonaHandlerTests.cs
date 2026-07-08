using EcoBrotes.Application.Ports;
using EcoBrotes.Application.Zonas.Command;
using EcoBrotes.Domain.Exceptions;
using EcoBrotes.Domain.ZonaUrbana.Entity;
using EcoBrotes.Domain.ZonaUrbana.Port;
using NSubstitute;

namespace EcoBrotes.Application.Tests.Zonas
{
    public class CreateZonaHandlerTests
    {
        private readonly IZonaUrbanaRepository _zonaRepository = Substitute.For<IZonaUrbanaRepository>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

        private CreateZonaHandler BuildHandler() => new(_zonaRepository, _unitOfWork);

        [Fact]
        public async Task Handle_WithUniqueName_AddsZonaAndSaves()
        {
            // Arrange
            _zonaRepository.GetByNameAsync("Zona Norte").Returns((ZonaUrbanaEntity)null!);
            var command = new CreateZonaCommand("Zona Norte");

            // Act
            await BuildHandler().Handle(command, CancellationToken.None);

            // Assert
            await _zonaRepository.Received(1).AddAsync(Arg.Is<ZonaUrbanaEntity>(z => z.Name == "Zona Norte"));
            await _unitOfWork.Received(1).SaveAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WithDuplicateName_ThrowsCoreBusinessException()
        {
            // Arrange
            _zonaRepository.GetByNameAsync("Zona Norte").Returns(new ZonaUrbanaEntity { Name = "Zona Norte" });
            var command = new CreateZonaCommand("Zona Norte");

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CoreBusinessException>(
                () => BuildHandler().Handle(command, CancellationToken.None));
            Assert.Contains("Ya existe", exception.Message);
            await _zonaRepository.DidNotReceive().AddAsync(Arg.Any<ZonaUrbanaEntity>());
            await _unitOfWork.DidNotReceive().SaveAsync(Arg.Any<CancellationToken>());
        }
    }
}
