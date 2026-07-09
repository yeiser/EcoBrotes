using EcoBrotes.Application.Jornadas.Query;
using EcoBrotes.Application.Tests.Common;
using EcoBrotes.Domain.EspecieArborea.Entity;
using EcoBrotes.Domain.EspecieArborea.Port;
using EcoBrotes.Domain.Exceptions;
using EcoBrotes.Domain.JornadasReforestacion.Entity;
using EcoBrotes.Domain.JornadasReforestacion.Port;
using EcoBrotes.Domain.ZonaUrbana.Entity;
using EcoBrotes.Domain.ZonaUrbana.Port;
using NSubstitute;

namespace EcoBrotes.Application.Tests.Jornadas
{
    public class GetJornadaByIdHandlerTests
    {
        private readonly IJornadaReforestacionRepository _jornadaRepository = Substitute.For<IJornadaReforestacionRepository>();
        private readonly IZonaUrbanaRepository _zonaRepository = Substitute.For<IZonaUrbanaRepository>();
        private readonly IEspecieArboreaRepository _especieRepository = Substitute.For<IEspecieArboreaRepository>();

        private GetJornadaByIdHandler BuildHandler() =>
            new(_jornadaRepository, _zonaRepository, _especieRepository);

        [Fact]
        public async Task Handle_WithExistingJornada_ReturnsDetailDtoWithSpecies()
        {
            // Arrange
            var id = Guid.NewGuid();
            var zonaId = Guid.NewGuid();
            var jornada = JornadaBuilder.WithState(JornadaState.ConvocatoriaAbierta, "REF-2026-050");
            jornada.Id = id;
            jornada.ZonaUrbanaId = zonaId;

            _jornadaRepository.GetByIdAsync(id).Returns(jornada);
            _zonaRepository.GetByIdAsync(zonaId).Returns(new ZonaUrbanaEntity { Id = zonaId, Name = "Zona Norte" });
            _especieRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(
                new EspecieArboreaEntity { Id = Guid.NewGuid(), Name = "Roble", ScientificName = "Quercus", MaxHeightMeters = 20m });

            // Act
            var dto = await BuildHandler().Handle(new GetJornadaByIdQuery { Id = id }, CancellationToken.None);

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal("REF-2026-050", dto.CodigoUnico);
            Assert.Equal("Zona Norte", dto.ZonaNombre);
            Assert.Equal("ConvocatoriaAbierta", dto.Estado);
            Assert.Single(dto.Especies);
            Assert.Equal("Roble", dto.Especies.First().Nombre);
        }

        [Fact]
        public async Task Handle_WithUnknownZona_UsesDesconocidaFallback()
        {
            // Arrange
            var id = Guid.NewGuid();
            var jornada = JornadaBuilder.WithState(JornadaState.ConvocatoriaAbierta, "REF-2026-051");
            jornada.Id = id;

            _jornadaRepository.GetByIdAsync(id).Returns(jornada);
            _zonaRepository.GetByIdAsync(Arg.Any<Guid>()).Returns((ZonaUrbanaEntity)null!);
            _especieRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(
                new EspecieArboreaEntity { Name = "Roble", ScientificName = "Quercus", MaxHeightMeters = 20m });

            // Act
            var dto = await BuildHandler().Handle(new GetJornadaByIdQuery { Id = id }, CancellationToken.None);

            // Assert
            Assert.Equal("Desconocida", dto.ZonaNombre);
        }

        [Fact]
        public async Task Handle_WithMissingJornada_ThrowsCoreBusinessException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _jornadaRepository.GetByIdAsync(id).Returns((JornadaReforestacion)null!);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CoreBusinessException>(
                () => BuildHandler().Handle(new GetJornadaByIdQuery { Id = id }, CancellationToken.None));
            Assert.Contains("no existe", exception.Message);
        }
    }
}
