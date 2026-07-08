using EcoBrotes.Domain.Common;
using EcoBrotes.Domain.Exceptions;
using DetalleArbolEntity = EcoBrotes.Domain.JornadasReforestacion.Entity.DetalleArbolJornada;
using EspecieArboreaEntity = EcoBrotes.Domain.EspecieArborea.Entity.EspecieArboreaEntity;

namespace EcoBrotes.Domain.Tests.JornadasReforestacion.Entity
{
    public class DetalleArbolJornadaEntityTests
    {
        #region Especie Validation

        [Fact]
        public void Detalle_Especie_WithNull_ThrowsException()
        {
            // Arrange
            var detalle = new DetalleArbolJornadaDataBuilder().Build();

            // Act & Assert
            var exception = Assert.Throws<RequiredException>(() => detalle.Especie = null!);
            Assert.Contains("no puede ser nula", exception.Message);
        }

        [Fact]
        public void Detalle_Especie_WithValidData_Success()
        {
            // Arrange
            var detalle = new DetalleArbolJornadaDataBuilder().Build();
            var especie = new EspecieArboreaEntity
            {
                Name = "Pino",
                ScientificName = "Pinus",
                MaxHeightMeters = 20m
            };

            // Act
            detalle.Especie = especie;

            // Assert
            Assert.Equal(especie, detalle.Especie);
        }

        #endregion

        #region Quantity Validation

        [Fact]
        public void Detalle_Quantity_WithZero_ThrowsException()
        {
            // Arrange
            var detalle = new DetalleArbolJornadaDataBuilder().Build();

            // Act & Assert
            var exception = Assert.Throws<RequiredException>(() => detalle.Quantity = 0);
            Assert.Contains("mayor a cero", exception.Message);
        }

        [Fact]
        public void Detalle_Quantity_WithNegativeValue_ThrowsException()
        {
            // Arrange
            var detalle = new DetalleArbolJornadaDataBuilder().Build();

            // Act & Assert
            var exception = Assert.Throws<RequiredException>(() => detalle.Quantity = -1);
            Assert.Contains("mayor a cero", exception.Message);
        }

        [Fact]
        public void Detalle_Quantity_WithValidData_Success()
        {
            // Arrange
            var detalle = new DetalleArbolJornadaDataBuilder().Build();

            // Act
            detalle.Quantity = 10;

            // Assert
            Assert.Equal(10, detalle.Quantity);
        }

        [Fact]
        public void Detalle_Quantity_WithMinimumValue_Success()
        {
            // Arrange
            var detalle = new DetalleArbolJornadaDataBuilder().Build();

            // Act
            detalle.Quantity = 1;

            // Assert
            Assert.Equal(1, detalle.Quantity);
        }

        [Fact]
        public void Detalle_Quantity_WithLargeValue_Success()
        {
            // Arrange
            var detalle = new DetalleArbolJornadaDataBuilder().Build();

            // Act
            detalle.Quantity = 1000;

            // Assert
            Assert.Equal(1000, detalle.Quantity);
        }

        #endregion

        #region Build with DataBuilder

        [Fact]
        public void Detalle_Build_WithDefaultValues_ReturnsValidEntity()
        {
            // Act
            var detalle = new DetalleArbolJornadaDataBuilder().Build();

            // Assert
            Assert.NotNull(detalle.Especie);
            Assert.True(detalle.Quantity > 0);
            Assert.NotNull(detalle.Id);
        }

        [Fact]
        public void Detalle_Build_WithCustomQuantity_ReturnsCorrectQuantity()
        {
            // Act
            var detalle = new DetalleArbolJornadaDataBuilder()
                .WithQuantity(25)
                .Build();

            // Assert
            Assert.Equal(25, detalle.Quantity);
        }

        [Fact]
        public void Detalle_Build_WithCustomEspecie_ReturnsCorrectEspecie()
        {
            // Arrange
            var especie = new EspecieArboreaEntity
            {
                Name = "Encino",
                ScientificName = "Quercus",
                MaxHeightMeters = 15m
            };

            // Act
            var detalle = new DetalleArbolJornadaDataBuilder()
                .WithEspecie(especie)
                .Build();

            // Assert
            Assert.Equal(especie.Name, detalle.Especie.Name);
            Assert.Equal(especie.ScientificName, detalle.Especie.ScientificName);
        }

        #endregion
    }
}
