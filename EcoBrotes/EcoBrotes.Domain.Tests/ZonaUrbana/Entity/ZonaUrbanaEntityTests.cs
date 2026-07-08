using EcoBrotes.Domain.Common;
using EcoBrotes.Domain.Exceptions;
using ZonaState = EcoBrotes.Domain.ZonaUrbana.Entity.ZonaState;

namespace EcoBrotes.Domain.Tests.ZonaUrbana.Entity
{
    public class ZonaUrbanaEntityTests
    {
        [Fact]
        public void Zona_Name_WithEmptyString_ThrowsException()
        {
            // Arrange
            var zona = new ZonaUrbanaDataBuilder().Build();

            // Act & Assert
            var exception = Assert.Throws<RequiredException>(() => zona.Name = "");
            Assert.Contains("no puede estar vacío", exception.Message);
        }

        [Fact]
        public void Zona_Name_WithNull_ThrowsException()
        {
            // Arrange
            var zona = new ZonaUrbanaDataBuilder().Build();

            // Act & Assert
            var exception = Assert.Throws<RequiredException>(() => zona.Name = null!);
            Assert.Contains("no puede estar vacío", exception.Message);
        }

        [Fact]
        public void Zona_Name_WithLessThan3Characters_ThrowsException()
        {
            // Arrange
            var zona = new ZonaUrbanaDataBuilder().Build();

            // Act & Assert
            var exception = Assert.Throws<RequiredException>(() => zona.Name = "AB");
            Assert.Contains("entre 3 y 100", exception.Message);
        }

        [Fact]
        public void Zona_Name_WithMoreThan100Characters_ThrowsException()
        {
            // Arrange
            var zona = new ZonaUrbanaDataBuilder().Build();
            var longName = new string('A', 101);

            // Act & Assert
            var exception = Assert.Throws<RequiredException>(() => zona.Name = longName);
            Assert.Contains("entre 3 y 100", exception.Message);
        }

        [Fact]
        public void Zona_Name_WithValidData_Success()
        {
            // Arrange
            var zona = new ZonaUrbanaDataBuilder().Build();

            // Act
            zona.Name = "Zona Reforestación Parque Central";

            // Assert
            Assert.Equal("Zona Reforestación Parque Central", zona.Name);
        }

        [Fact]
        public void Zona_Name_WithExactly3Characters_Success()
        {
            // Arrange
            var zona = new ZonaUrbanaDataBuilder().Build();

            // Act
            zona.Name = "ABC";

            // Assert
            Assert.Equal("ABC", zona.Name);
        }

        [Fact]
        public void Zona_Name_WithExactly100Characters_Success()
        {
            // Arrange
            var zona = new ZonaUrbanaDataBuilder().Build();
            var name100 = new string('A', 100);

            // Act
            zona.Name = name100;

            // Assert
            Assert.Equal(name100, zona.Name);
        }

        [Fact]
        public void Zona_State_DefaultValue_IsActive()
        {
            // Arrange
            var zona = new ZonaUrbanaDataBuilder().Build();

            // Assert
            Assert.Equal(ZonaState.Activa, zona.State);
        }

        [Fact]
        public void Zona_Deactivate_ChangesStateToInactive()
        {
            // Arrange
            var zona = new ZonaUrbanaDataBuilder().Build();
            Assert.Equal(ZonaState.Activa, zona.State);

            // Act
            zona.Deactivate();

            // Assert
            Assert.Equal(ZonaState.Inactiva, zona.State);
        }

        [Fact]
        public void Zona_Deactivate_CanBeCalledMultipleTimes()
        {
            // Arrange
            var zona = new ZonaUrbanaDataBuilder().Build();

            // Act
            zona.Deactivate();
            zona.Deactivate();

            // Assert
            Assert.Equal(ZonaState.Inactiva, zona.State);
        }
    }
}
