using EcoBrotes.Application.Ports;
using EcoBrotes.Application.Tests.Common;
using EcoBrotes.Application.Zonas.Command;
using EcoBrotes.Domain.Exceptions;
using EcoBrotes.Domain.JornadasReforestacion.Entity;
using EcoBrotes.Domain.JornadasReforestacion.Port;
using EcoBrotes.Domain.ZonaUrbana.Entity;
using EcoBrotes.Domain.ZonaUrbana.Port;
using NSubstitute;

namespace EcoBrotes.Application.Tests.Zonas
{
    public class DeleteZonaHandlerTests
    {
        private readonly IZonaUrbanaRepository _zonaRepository = Substitute.For<IZonaUrbanaRepository>();
        private readonly IJornadaReforestacionRepository _jornadaRepository = Substitute.For<IJornadaReforestacionRepository>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

        private DeleteZonaHandler BuildHandler() =>
            new(_zonaRepository, _jornadaRepository, _unitOfWork);

        [Fact]
        public async Task Handle_WithNoActiveReferences_DeactivatesAndSaves()
        {
            // Arrange
            var zona = new ZonaUrbanaEntity { Name = "Zona Norte" };
            var id = Guid.NewGuid();
            _zonaRepository.GetByIdAsync(id).Returns(zona);
            _jornadaRepository.GetByZonaIdAsync(id).Returns(Enumerable.Empty<JornadaReforestacion>());
            var command = new DeleteZonaCommand(id);

            // Act
            await BuildHandler().Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(ZonaState.Inactiva, zona.State);
            await _unitOfWork.Received(1).SaveAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WithMissingZona_ThrowsRequiredException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _zonaRepository.GetByIdAsync(id).Returns((ZonaUrbanaEntity)null!);
            var command = new DeleteZonaCommand(id);

            // Act & Assert
            await Assert.ThrowsAsync<RequiredException>(
                () => BuildHandler().Handle(command, CancellationToken.None));
            await _unitOfWork.DidNotReceive().SaveAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WithAlreadyInactiveZona_ThrowsCoreBusinessException()
        {
            // Arrange
            var zona = new ZonaUrbanaEntity { Name = "Zona Norte" };
            zona.Deactivate();
            var id = Guid.NewGuid();
            _zonaRepository.GetByIdAsync(id).Returns(zona);
            var command = new DeleteZonaCommand(id);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CoreBusinessException>(
                () => BuildHandler().Handle(command, CancellationToken.None));
            Assert.Contains("ya se encuentra desactivada", exception.Message);
        }

        [Trait("ReglaNegocio", "RB-09")]
        [Fact]
        public async Task Handle_WithActiveJornadaReference_ThrowsCoreBusinessException()
        {
            // Arrange
            var zona = new ZonaUrbanaEntity { Name = "Zona Norte" };
            var id = Guid.NewGuid();
            _zonaRepository.GetByIdAsync(id).Returns(zona);
            var activeJornada = JornadaBuilder.WithState(JornadaState.CupoCompleto, "REF-2026-020");
            _jornadaRepository.GetByZonaIdAsync(id).Returns(new[] { activeJornada });
            var command = new DeleteZonaCommand(id);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CoreBusinessException>(
                () => BuildHandler().Handle(command, CancellationToken.None));
            Assert.Contains("REF-2026-020", exception.Message);
            await _unitOfWork.DidNotReceive().SaveAsync(Arg.Any<CancellationToken>());
        }
    }
}
