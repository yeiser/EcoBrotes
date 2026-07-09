using EcoBrotes.Application.Especies.Command;
using EcoBrotes.Application.Ports;
using EcoBrotes.Domain.EspecieArborea.Entity;
using EcoBrotes.Domain.Exceptions;
using NSubstitute;

namespace EcoBrotes.Application.Tests.Especies
{
    public class UpdateEspecieHandlerTests
    {
        private readonly IRepository<EspecieArboreaEntity> _especieRepository = Substitute.For<IRepository<EspecieArboreaEntity>>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

        private UpdateEspecieHandler BuildHandler() => new(_especieRepository, _unitOfWork);

        [Fact]
        public async Task Handle_WithActiveEspecie_UpdatesFieldsAndSaves()
        {
            // Arrange
            var especie = new EspecieArboreaEntity { Name = "Roble", ScientificName = "Quercus", MaxHeightMeters = 20m };
            var id = Guid.NewGuid();
            _especieRepository.GetByIdAsync(id).Returns(especie);
            var command = new UpdateEspecieCommand(id, "Ceiba", "Ceiba pentandra", 40m);

            // Act
            await BuildHandler().Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("Ceiba", especie.Name);
            Assert.Equal("Ceiba pentandra", especie.ScientificName);
            Assert.Equal(40m, especie.MaxHeightMeters);
            await _unitOfWork.Received(1).SaveAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WithMissingEspecie_ThrowsRequiredException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _especieRepository.GetByIdAsync(id).Returns((EspecieArboreaEntity)null!);
            var command = new UpdateEspecieCommand(id, "Ceiba", "Ceiba pentandra", 40m);

            // Act & Assert
            await Assert.ThrowsAsync<RequiredException>(
                () => BuildHandler().Handle(command, CancellationToken.None));
            await _unitOfWork.DidNotReceive().SaveAsync(Arg.Any<CancellationToken>());
        }

        [Trait("ReglaNegocio", "RB-06")]
        [Fact]
        public async Task Handle_WithInactiveEspecie_ThrowsCoreBusinessException()
        {
            // Arrange
            var especie = new EspecieArboreaEntity { Name = "Roble", ScientificName = "Quercus", MaxHeightMeters = 20m };
            especie.Deactivate();
            var id = Guid.NewGuid();
            _especieRepository.GetByIdAsync(id).Returns(especie);
            var command = new UpdateEspecieCommand(id, "Ceiba", "Ceiba pentandra", 40m);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CoreBusinessException>(
                () => BuildHandler().Handle(command, CancellationToken.None));
            Assert.Contains("desactivada", exception.Message);
            await _unitOfWork.DidNotReceive().SaveAsync(Arg.Any<CancellationToken>());
        }
    }
}
