using EcoBrotes.Application.Especies.Command;
using EcoBrotes.Application.Ports;
using EcoBrotes.Domain.EspecieArborea.Entity;
using EcoBrotes.Domain.EspecieArborea.Port;
using EcoBrotes.Domain.Exceptions;
using NSubstitute;

namespace EcoBrotes.Application.Tests.Especies
{
    public class CreateEspecieHandlerTests
    {
        private readonly IEspecieArboreaRepository _especieRepository = Substitute.For<IEspecieArboreaRepository>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

        private CreateEspecieHandler BuildHandler() => new(_especieRepository, _unitOfWork);

        [Fact]
        public async Task Handle_WithUniqueName_AddsEspecieAndSaves()
        {
            // Arrange
            _especieRepository.GetByNameAsync("Roble").Returns((EspecieArboreaEntity)null!);
            var command = new CreateEspecieCommand("Roble", "Quercus", 20m);

            // Act
            await BuildHandler().Handle(command, CancellationToken.None);

            // Assert
            await _especieRepository.Received(1).AddAsync(Arg.Is<EspecieArboreaEntity>(e =>
                e.Name == "Roble" && e.ScientificName == "Quercus" && e.MaxHeightMeters == 20m));
            await _unitOfWork.Received(1).SaveAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WithDuplicateName_ThrowsCoreBusinessException()
        {
            // Arrange
            var existing = new EspecieArboreaEntity { Name = "Roble", ScientificName = "Quercus", MaxHeightMeters = 20m };
            _especieRepository.GetByNameAsync("Roble").Returns(existing);
            var command = new CreateEspecieCommand("Roble", "Quercus", 20m);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CoreBusinessException>(
                () => BuildHandler().Handle(command, CancellationToken.None));
            Assert.Contains("Ya existe", exception.Message);
            await _especieRepository.DidNotReceive().AddAsync(Arg.Any<EspecieArboreaEntity>());
            await _unitOfWork.DidNotReceive().SaveAsync(Arg.Any<CancellationToken>());
        }
    }
}
