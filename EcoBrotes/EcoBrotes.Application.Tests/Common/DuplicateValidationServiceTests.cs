using EcoBrotes.Application.Common;
using EcoBrotes.Domain.Exceptions;
using EcoBrotes.Domain.ZonaUrbana.Entity;

namespace EcoBrotes.Application.Tests.Common
{
    public class DuplicateValidationServiceTests
    {
        [Fact]
        public async Task ValidateNoDuplicateNameAsync_WhenNoExistingEntity_DoesNotThrow()
        {
            // Arrange
            Func<Task<ZonaUrbanaEntity?>> getByName = () => Task.FromResult<ZonaUrbanaEntity?>(null);

            // Act & Assert (no exception)
            await DuplicateValidationService.ValidateNoDuplicateNameAsync(getByName, "una zona urbana", "Zona Norte");
        }

        [Fact]
        public async Task ValidateNoDuplicateNameAsync_WhenEntityExists_ThrowsCoreBusinessException()
        {
            // Arrange
            var existing = new ZonaUrbanaEntity { Name = "Zona Norte" };
            Func<Task<ZonaUrbanaEntity?>> getByName = () => Task.FromResult<ZonaUrbanaEntity?>(existing);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CoreBusinessException>(
                () => DuplicateValidationService.ValidateNoDuplicateNameAsync(getByName, "una zona urbana", "Zona Norte"));
            Assert.Contains("Ya existe una zona urbana con el nombre 'Zona Norte'", exception.Message);
        }
    }
}
