using EcoBrotes.Application.Common;
using EcoBrotes.Domain.Exceptions;
using EcoBrotes.Domain.ZonaUrbana.Entity;

namespace EcoBrotes.Application.Tests.Common
{
    public class EntityMapperTests
    {
        [Fact]
        public void MapEntity_WithNonNullEntity_ReturnsMappedDto()
        {
            // Arrange
            var zona = new ZonaUrbanaEntity { Id = Guid.NewGuid(), Name = "Zona Norte" };

            // Act
            var name = EntityMapper.MapEntity(zona, "la zona", z => z.Name);

            // Assert
            Assert.Equal("Zona Norte", name);
        }

        [Fact]
        public void MapEntity_WithNullEntity_ThrowsCoreBusinessException()
        {
            // Act & Assert
            var exception = Assert.Throws<CoreBusinessException>(
                () => EntityMapper.MapEntity<ZonaUrbanaEntity, string>(null!, "la zona", z => z.Name));
            Assert.Contains("la zona no existe", exception.Message);
        }

        [Fact]
        public void MapEntities_WithCollection_MapsEveryElement()
        {
            // Arrange
            var zonas = new[]
            {
                new ZonaUrbanaEntity { Name = "Zona Norte" },
                new ZonaUrbanaEntity { Name = "Zona Sur" }
            };

            // Act
            var names = EntityMapper.MapEntities(zonas, z => z.Name);

            // Assert
            Assert.Equal(new[] { "Zona Norte", "Zona Sur" }, names);
        }
    }
}
