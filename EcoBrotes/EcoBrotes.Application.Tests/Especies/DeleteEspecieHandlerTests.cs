using EcoBrotes.Application.Especies.Command;
using EcoBrotes.Application.Ports;
using EcoBrotes.Domain.EspecieArborea.Entity;
using EcoBrotes.Domain.EspecieArborea.Port;
using EcoBrotes.Domain.Exceptions;
using EcoBrotes.Domain.JornadasReforestacion.Entity;
using EcoBrotes.Domain.JornadasReforestacion.Port;
using EcoBrotes.Application.Tests.Common;
using NSubstitute;

namespace EcoBrotes.Application.Tests.Especies
{
    public class DeleteEspecieHandlerTests
    {
        private readonly IEspecieArboreaRepository _especieRepository = Substitute.For<IEspecieArboreaRepository>();
        private readonly IJornadaReforestacionRepository _jornadaRepository = Substitute.For<IJornadaReforestacionRepository>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

        private DeleteEspecieHandler BuildHandler() =>
            new(_especieRepository, _jornadaRepository, _unitOfWork);

        [Fact]
        public async Task Handle_WithNoActiveReferences_DeactivatesAndSaves()
        {
            // Arrange
            var especie = new EspecieArboreaEntity { Name = "Roble", ScientificName = "Quercus", MaxHeightMeters = 20m };
            var id = Guid.NewGuid();
            _especieRepository.GetByIdAsync(id).Returns(especie);
            _jornadaRepository.GetByEspecieArboreaIdAsync(id).Returns(Enumerable.Empty<JornadaReforestacion>());
            var command = new DeleteEspecieCommand(id);

            // Act
            await BuildHandler().Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(EspecieState.Inactiva, especie.State);
            await _unitOfWork.Received(1).SaveAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WithMissingEspecie_ThrowsRequiredException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _especieRepository.GetByIdAsync(id).Returns((EspecieArboreaEntity)null!);
            var command = new DeleteEspecieCommand(id);

            // Act & Assert
            await Assert.ThrowsAsync<RequiredException>(
                () => BuildHandler().Handle(command, CancellationToken.None));
            await _unitOfWork.DidNotReceive().SaveAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WithAlreadyInactiveEspecie_ThrowsCoreBusinessException()
        {
            // Arrange
            var especie = new EspecieArboreaEntity { Name = "Roble", ScientificName = "Quercus", MaxHeightMeters = 20m };
            especie.Deactivate();
            var id = Guid.NewGuid();
            _especieRepository.GetByIdAsync(id).Returns(especie);
            var command = new DeleteEspecieCommand(id);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CoreBusinessException>(
                () => BuildHandler().Handle(command, CancellationToken.None));
            Assert.Contains("ya se encuentra desactivada", exception.Message);
        }

        [Fact]
        public async Task Handle_WithActiveJornadaReference_ThrowsCoreBusinessException()
        {
            // Arrange
            var especie = new EspecieArboreaEntity { Name = "Roble", ScientificName = "Quercus", MaxHeightMeters = 20m };
            var id = Guid.NewGuid();
            _especieRepository.GetByIdAsync(id).Returns(especie);
            var activeJornada = JornadaBuilder.WithState(JornadaState.ConvocatoriaAbierta, "REF-2026-010");
            _jornadaRepository.GetByEspecieArboreaIdAsync(id).Returns(new[] { activeJornada });
            var command = new DeleteEspecieCommand(id);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CoreBusinessException>(
                () => BuildHandler().Handle(command, CancellationToken.None));
            Assert.Contains("REF-2026-010", exception.Message);
            await _unitOfWork.DidNotReceive().SaveAsync(Arg.Any<CancellationToken>());
        }
    }
}
