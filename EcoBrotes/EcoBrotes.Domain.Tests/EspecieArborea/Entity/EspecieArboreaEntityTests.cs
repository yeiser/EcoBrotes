using EcoBrotes.Domain.Common;
using EcoBrotes.Domain.Exceptions;
using EspecieState = EcoBrotes.Domain.EspecieArborea.Entity.EspecieState;

namespace EcoBrotes.Domain.Tests.EspecieArborea.Entity
{
    public class EspecieArboreaEntityTests
    {
        #region Name Validation

        [Fact]
        public void Especie_Name_WithEmptyString_ThrowsException()
        {
            // Arrange
            var especie = new EspecieArboreaDataBuilder().Build();

            // Act & Assert
            var exception = Assert.Throws<RequiredException>(() => especie.Name = "");
            Assert.Contains("no puede estar vacío", exception.Message);
        }

        [Fact]
        public void Especie_Name_WithNull_ThrowsException()
        {
            // Arrange
            var especie = new EspecieArboreaDataBuilder().Build();

            // Act & Assert
            var exception = Assert.Throws<RequiredException>(() => especie.Name = null!);
            Assert.Contains("no puede estar vacío", exception.Message);
        }

        [Fact]
        public void Especie_Name_WithLessThan3Characters_ThrowsException()
        {
            // Arrange
            var especie = new EspecieArboreaDataBuilder().Build();

            // Act & Assert
            var exception = Assert.Throws<RequiredException>(() => especie.Name = "AB");
            Assert.Contains("entre 3 y 150", exception.Message);
        }

        [Fact]
        public void Especie_Name_WithMoreThan150Characters_ThrowsException()
        {
            // Arrange
            var especie = new EspecieArboreaDataBuilder().Build();
            var longName = new string('A', 151);

            // Act & Assert
            var exception = Assert.Throws<RequiredException>(() => especie.Name = longName);
            Assert.Contains("entre 3 y 150", exception.Message);
        }

        [Fact]
        public void Especie_Name_WithValidData_Success()
        {
            // Arrange
            var especie = new EspecieArboreaDataBuilder().Build();

            // Act
            especie.Name = "Encino de Sierra";

            // Assert
            Assert.Equal("Encino de Sierra", especie.Name);
        }

        [Fact]
        public void Especie_Name_WithExactly3Characters_Success()
        {
            // Arrange
            var especie = new EspecieArboreaDataBuilder().Build();

            // Act
            especie.Name = "ABC";

            // Assert
            Assert.Equal("ABC", especie.Name);
        }

        [Fact]
        public void Especie_Name_WithExactly150Characters_Success()
        {
            // Arrange
            var especie = new EspecieArboreaDataBuilder().Build();
            var name150 = new string('A', 150);

            // Act
            especie.Name = name150;

            // Assert
            Assert.Equal(name150, especie.Name);
        }

        #endregion

        #region ScientificName Validation

        [Fact]
        public void Especie_ScientificName_WithEmptyString_ThrowsException()
        {
            // Arrange
            var especie = new EspecieArboreaDataBuilder().Build();

            // Act & Assert
            var exception = Assert.Throws<RequiredException>(() => especie.ScientificName = "");
            Assert.Contains("no puede estar vacío", exception.Message);
        }

        [Fact]
        public void Especie_ScientificName_WithNull_ThrowsException()
        {
            // Arrange
            var especie = new EspecieArboreaDataBuilder().Build();

            // Act & Assert
            var exception = Assert.Throws<RequiredException>(() => especie.ScientificName = null!);
            Assert.Contains("no puede estar vacío", exception.Message);
        }

        [Fact]
        public void Especie_ScientificName_WithValidData_Success()
        {
            // Arrange
            var especie = new EspecieArboreaDataBuilder().Build();

            // Act
            especie.ScientificName = "Quercus rugosa";

            // Assert
            Assert.Equal("Quercus rugosa", especie.ScientificName);
        }

        #endregion

        #region MaxHeightMeters Validation

        [Fact]
        public void Especie_MaxHeightMeters_WithZero_ThrowsException()
        {
            // Arrange
            var especie = new EspecieArboreaDataBuilder().Build();

            // Act & Assert
            var exception = Assert.Throws<RequiredException>(() => especie.MaxHeightMeters = 0);
            Assert.Contains("mayor a cero", exception.Message);
        }

        [Fact]
        public void Especie_MaxHeightMeters_WithNegativeValue_ThrowsException()
        {
            // Arrange
            var especie = new EspecieArboreaDataBuilder().Build();

            // Act & Assert
            var exception = Assert.Throws<RequiredException>(() => especie.MaxHeightMeters = -5m);
            Assert.Contains("mayor a cero", exception.Message);
        }

        [Fact]
        public void Especie_MaxHeightMeters_WithValidData_Success()
        {
            // Arrange
            var especie = new EspecieArboreaDataBuilder().Build();

            // Act
            especie.MaxHeightMeters = 30.5m;

            // Assert
            Assert.Equal(30.5m, especie.MaxHeightMeters);
        }

        [Fact]
        public void Especie_MaxHeightMeters_WithDecimalValue_Success()
        {
            // Arrange
            var especie = new EspecieArboreaDataBuilder().Build();

            // Act
            especie.MaxHeightMeters = 1.5m;

            // Assert
            Assert.Equal(1.5m, especie.MaxHeightMeters);
        }

        [Fact]
        public void Especie_MaxHeightMeters_WithMinimumPositiveValue_Success()
        {
            // Arrange
            var especie = new EspecieArboreaDataBuilder().Build();

            // Act
            especie.MaxHeightMeters = 0.01m;

            // Assert
            Assert.Equal(0.01m, especie.MaxHeightMeters);
        }

        #endregion

        #region State Tests

        [Fact]
        public void Especie_State_DefaultValue_IsActive()
        {
            // Arrange
            var especie = new EspecieArboreaDataBuilder().Build();

            // Assert
            Assert.Equal(EspecieState.Activa, especie.State);
        }

        [Fact]
        public void Especie_Deactivate_ChangesStateToInactive()
        {
            // Arrange
            var especie = new EspecieArboreaDataBuilder().Build();
            Assert.Equal(EspecieState.Activa, especie.State);

            // Act
            especie.Deactivate();

            // Assert
            Assert.Equal(EspecieState.Inactiva, especie.State);
        }

        [Fact]
        public void Especie_Deactivate_CanBeCalledMultipleTimes()
        {
            // Arrange
            var especie = new EspecieArboreaDataBuilder().Build();

            // Act
            especie.Deactivate();
            especie.Deactivate();

            // Assert
            Assert.Equal(EspecieState.Inactiva, especie.State);
        }

        #endregion
    }
}
