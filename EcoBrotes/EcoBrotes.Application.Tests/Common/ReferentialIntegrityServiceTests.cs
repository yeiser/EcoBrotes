using EcoBrotes.Application.Common;
using EcoBrotes.Domain.Exceptions;
using EcoBrotes.Domain.JornadasReforestacion.Entity;
using EcoBrotes.Domain.JornadasReforestacion.Port;
using NSubstitute;

namespace EcoBrotes.Application.Tests.Common
{
    public class ReferentialIntegrityServiceTests
    {
        private readonly ReferentialIntegrityService _service =
            new(Substitute.For<IJornadaReforestacionRepository>());

        [Fact]
        public async Task ValidateNoActiveReferencesAsync_WithNoJornadas_DoesNotThrow()
        {
            // Arrange
            Func<Task<IEnumerable<JornadaReforestacion>>> getReferenced =
                () => Task.FromResult(Enumerable.Empty<JornadaReforestacion>());

            // Act & Assert (no exception)
            await _service.ValidateNoActiveReferencesAsync(getReferenced, "la especie", Guid.NewGuid());
        }

        [Fact]
        public async Task ValidateNoActiveReferencesAsync_WithOnlyClosedJornadas_DoesNotThrow()
        {
            // Arrange
            var jornadas = new[]
            {
                JornadaBuilder.WithState(JornadaState.Finalizada, "REF-2026-030"),
                JornadaBuilder.WithState(JornadaState.Cancelada, "REF-2026-031")
            };
            Func<Task<IEnumerable<JornadaReforestacion>>> getReferenced =
                () => Task.FromResult<IEnumerable<JornadaReforestacion>>(jornadas);

            // Act & Assert (no exception)
            await _service.ValidateNoActiveReferencesAsync(getReferenced, "la especie", Guid.NewGuid());
        }

        [Fact]
        public async Task ValidateNoActiveReferencesAsync_WithActiveJornada_ThrowsWithCodes()
        {
            // Arrange
            var jornadas = new[]
            {
                JornadaBuilder.WithState(JornadaState.ConvocatoriaAbierta, "REF-2026-040"),
                JornadaBuilder.WithState(JornadaState.Finalizada, "REF-2026-041")
            };
            Func<Task<IEnumerable<JornadaReforestacion>>> getReferenced =
                () => Task.FromResult<IEnumerable<JornadaReforestacion>>(jornadas);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CoreBusinessException>(
                () => _service.ValidateNoActiveReferencesAsync(getReferenced, "la especie", Guid.NewGuid()));
            Assert.Contains("REF-2026-040", exception.Message);
            Assert.DoesNotContain("REF-2026-041", exception.Message);
        }
    }
}
