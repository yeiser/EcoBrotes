using EcoBrotes.Application.Jornadas.Command;
using EcoBrotes.Application.Jornadas.Command.Factory;
using EcoBrotes.Domain.EspecieArborea.Entity;
using EcoBrotes.Domain.EspecieArborea.Port;
using EcoBrotes.Domain.Exceptions;
using EcoBrotes.Domain.ZonaUrbana.Entity;
using EcoBrotes.Domain.ZonaUrbana.Port;
using NSubstitute;

namespace EcoBrotes.Application.Tests.Jornadas
{
    public class JornadaFactoryTests
    {
        private readonly IZonaUrbanaRepository _zonaRepository = Substitute.For<IZonaUrbanaRepository>();
        private readonly IEspecieArboreaRepository _especieRepository = Substitute.For<IEspecieArboreaRepository>();

        private JornadaFactory BuildFactory() => new(_zonaRepository, _especieRepository);

        private static CreateJornadaCommand Command(Guid zonaId, Guid especieId, int quantity = 10, int treeMeta = 10, int volunteerCapacity = 2)
            => new(zonaId, "Jornada Test", DateTime.UtcNow.AddDays(14), treeMeta, volunteerCapacity,
                   new[] { new DetalleEspecieCommand(especieId, quantity) });

        private static ZonaUrbanaEntity ActiveZona(Guid id) => new() { Id = id, Name = "Zona Norte" };
        private static EspecieArboreaEntity ActiveEspecie(Guid id) => new() { Id = id, Name = "Roble", ScientificName = "Quercus", MaxHeightMeters = 20m };

        [Fact]
        public async Task CreateAsync_WithValidData_SetsZonaUrbanaIdAndCode()
        {
            // Arrange
            var zonaId = Guid.NewGuid();
            var especieId = Guid.NewGuid();
            _zonaRepository.GetByIdAsync(zonaId).Returns(ActiveZona(zonaId));
            _especieRepository.GetByIdAsync(especieId).Returns(ActiveEspecie(especieId));

            // Act
            var jornada = await BuildFactory().CreateAsync(Command(zonaId, especieId), "REF-2026-001");

            // Assert
            Assert.Equal(zonaId, jornada.ZonaUrbanaId);
            Assert.Equal("REF-2026-001", jornada.CodigoUnico);
            Assert.Single(jornada.DetalleArboles);
        }

        [Fact]
        public async Task CreateAsync_WithMissingZona_ThrowsRequiredException()
        {
            // Arrange
            var zonaId = Guid.NewGuid();
            _zonaRepository.GetByIdAsync(zonaId).Returns((ZonaUrbanaEntity)null!);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<RequiredException>(
                () => BuildFactory().CreateAsync(Command(zonaId, Guid.NewGuid()), "REF"));
            Assert.Contains("no existe", ex.Message);
        }

        [Trait("ReglaNegocio", "RB-06")]
        [Fact]
        public async Task CreateAsync_WithInactiveZona_ThrowsCoreBusinessException()
        {
            // Arrange
            var zonaId = Guid.NewGuid();
            var zona = ActiveZona(zonaId);
            zona.Deactivate();
            _zonaRepository.GetByIdAsync(zonaId).Returns(zona);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<CoreBusinessException>(
                () => BuildFactory().CreateAsync(Command(zonaId, Guid.NewGuid()), "REF"));
            Assert.Contains("desactivada", ex.Message);
        }

        [Trait("ReglaNegocio", "RB-06")]
        [Fact]
        public async Task CreateAsync_WithInactiveEspecie_ThrowsCoreBusinessException()
        {
            // Arrange
            var zonaId = Guid.NewGuid();
            var especieId = Guid.NewGuid();
            _zonaRepository.GetByIdAsync(zonaId).Returns(ActiveZona(zonaId));
            var especie = ActiveEspecie(especieId);
            especie.Deactivate();
            _especieRepository.GetByIdAsync(especieId).Returns(especie);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<CoreBusinessException>(
                () => BuildFactory().CreateAsync(Command(zonaId, especieId), "REF"));
            Assert.Contains("desactivada", ex.Message);
        }

        [Trait("ReglaNegocio", "RB-03")]
        [Fact]
        public async Task CreateAsync_WithQuantityLessThanOne_ThrowsCoreBusinessException()
        {
            // Arrange
            var zonaId = Guid.NewGuid();
            var especieId = Guid.NewGuid();
            _zonaRepository.GetByIdAsync(zonaId).Returns(ActiveZona(zonaId));
            _especieRepository.GetByIdAsync(especieId).Returns(ActiveEspecie(especieId));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<CoreBusinessException>(
                () => BuildFactory().CreateAsync(Command(zonaId, especieId, quantity: 0), "REF"));
            Assert.Contains("al menos 1", ex.Message);
        }

        [Trait("ReglaNegocio", "RB-06")]
        [Fact]
        public async Task ValidateUpdateAsync_WithInactiveZona_ThrowsCoreBusinessException()
        {
            // Arrange
            var zonaId = Guid.NewGuid();
            var zona = ActiveZona(zonaId);
            zona.Deactivate();
            _zonaRepository.GetByIdAsync(zonaId).Returns(zona);

            // Act & Assert
            await Assert.ThrowsAsync<CoreBusinessException>(
                () => BuildFactory().ValidateUpdateAsync(zonaId, new[] { new DetalleEspecieCommand(Guid.NewGuid(), 5) }));
        }

        [Fact]
        public async Task ValidateUpdateAsync_WithValidData_ReturnsMappedDetalles()
        {
            // Arrange
            var zonaId = Guid.NewGuid();
            var especieId = Guid.NewGuid();
            _zonaRepository.GetByIdAsync(zonaId).Returns(ActiveZona(zonaId));
            _especieRepository.GetByIdAsync(especieId).Returns(ActiveEspecie(especieId));

            // Act
            var detalles = await BuildFactory().ValidateUpdateAsync(zonaId, new[] { new DetalleEspecieCommand(especieId, 5) });

            // Assert
            Assert.Single(detalles);
            Assert.Equal(5, detalles[0].Quantity);
        }
    }
}
