using EcoBrotes.Domain.Common;
using EcoBrotes.Domain.Exceptions;
using EspecieArboreaEntity = EcoBrotes.Domain.EspecieArborea.Entity.EspecieArboreaEntity;
using DetalleArbolEntity = EcoBrotes.Domain.JornadasReforestacion.Entity.DetalleArbolJornada;
using JornadaStateEnum = EcoBrotes.Domain.JornadasReforestacion.Entity.JornadaState;
using ZonaUrbanaEntity = EcoBrotes.Domain.ZonaUrbana.Entity.ZonaUrbanaEntity;
using ZonaUrbanaDataBuilder = EcoBrotes.Domain.Tests.ZonaUrbana.Entity.ZonaUrbanaDataBuilder;
using EspecieArboreaDataBuilder = EcoBrotes.Domain.Tests.EspecieArborea.Entity.EspecieArboreaDataBuilder;
using JornadaReforestacion = EcoBrotes.Domain.JornadasReforestacion.Entity.JornadaReforestacion;

namespace EcoBrotes.Domain.Tests.JornadasReforestacion.Entity
{
    public class JornadaReforestacionTests
    {
        [Fact]
        public void Jornada_Initialize_WithDateLessThan7Days_ThrowsException()
        {
            var jornada = new JornadaReforestacionDataBuilder()
                .WithScheduledDate(DateTime.UtcNow.AddDays(3))
                .WithTreeMeta(10)
                .WithDetalleArboles([new DetalleArbolJornadaDataBuilder().WithQuantity(10).Build()])
                .Build();

            var exception = Assert.Throws<CoreBusinessException>(() =>
                jornada.Initialize("REF-2026-001", DateTime.UtcNow.AddDays(3), 10,
                    new List<DetalleArbolEntity> { new DetalleArbolJornadaDataBuilder().WithQuantity(10).Build() }));

            Assert.Contains("7", exception.Message);
            Assert.Contains("men", exception.Message.ToLower());
        }

        [Fact]
        public void Jornada_Initialize_WithInconsistentMeta_ThrowsException()
        {
            var jornada = new JornadaReforestacionDataBuilder()
                .WithTreeMeta(10)
                .Build();

            var exception = Assert.Throws<CoreBusinessException>(() =>
                jornada.Initialize("REF-2026-001", DateTime.UtcNow.AddDays(14), 10,
                    new List<DetalleArbolEntity> { new DetalleArbolJornadaDataBuilder().WithQuantity(7).Build() }));

            Assert.Contains("no coincide", exception.Message);
            Assert.Contains("3", exception.Message);
        }

        [Fact]
        public void Jornada_Initialize_WithInsufficientVolunteers_ThrowsException()
        {
            var jornada = new JornadaReforestacionDataBuilder()
                .WithTreeMeta(10)
                .WithVolunteerCapacity(1)
                .Build();

            var exception = Assert.Throws<CoreBusinessException>(() =>
                jornada.Initialize("REF-2026-001", DateTime.UtcNow.AddDays(14), 10,
                    new List<DetalleArbolEntity> { new DetalleArbolJornadaDataBuilder().WithQuantity(10).Build() }));

            Assert.Contains("cupo", exception.Message);
            Assert.Contains("2", exception.Message);
        }

        [Fact]
        public void Jornada_Initialize_WithDuplicateSpecies_ThrowsException()
        {
            var especie = new EspecieArboreaEntity { Name = "Pino", ScientificName = "Pinus", MaxHeightMeters = 20 };
            var detalle1 = new DetalleArbolJornadaDataBuilder().WithEspecie(especie).WithQuantity(5).Build();
            var detalle2 = new DetalleArbolJornadaDataBuilder().WithEspecie(especie).WithQuantity(5).Build();

            var jornada = new JornadaReforestacionDataBuilder().WithTreeMeta(10).Build();

            var exception = Assert.Throws<CoreBusinessException>(() =>
                jornada.Initialize("REF-2026-001", DateTime.UtcNow.AddDays(14), 10,
                    new List<DetalleArbolEntity> { detalle1, detalle2 }));

            Assert.Contains("Pino", exception.Message);
            Assert.Contains("consolide", exception.Message.ToLower());
        }

        [Fact]
        public void Jornada_Initialize_WithValidData_Success()
        {
            var jornada = new JornadaReforestacionDataBuilder()
                .WithTreeMeta(10)
                .WithVolunteerCapacity(2)
                .Build();

            var detalle = new List<DetalleArbolEntity>
            {
                new DetalleArbolJornadaDataBuilder().WithQuantity(6).Build(),
                new DetalleArbolJornadaDataBuilder().WithQuantity(4).Build()
            };

            jornada.Initialize("REF-2026-001", DateTime.UtcNow.AddDays(14), 10, detalle);

            Assert.Equal(JornadaStateEnum.ConvocatoriaAbierta, jornada.State);
            Assert.Equal("REF-2026-001", jornada.CodigoUnico);
            Assert.Equal(10, jornada.TotalAssignedTrees);
        }

        [Fact]
        public void Jornada_Cancel_Success()
        {
            var jornada = new JornadaReforestacionDataBuilder().Build();
            jornada.Initialize("REF-2026-001", DateTime.UtcNow.AddDays(14), 10,
                new List<DetalleArbolEntity> { new DetalleArbolJornadaDataBuilder().WithQuantity(10).Build() });

            jornada.Cancel();

            Assert.Equal(JornadaStateEnum.Cancelada, jornada.State);
        }

        [Fact]
        public void Jornada_Cancel_WhenAlreadyCancelled_ThrowsException()
        {
            var jornada = new JornadaReforestacionDataBuilder().Build();
            jornada.Initialize("REF-2026-001", DateTime.UtcNow.AddDays(14), 10,
                new List<DetalleArbolEntity> { new DetalleArbolJornadaDataBuilder().WithQuantity(10).Build() });
            jornada.Cancel();

            var exception = Assert.Throws<CoreBusinessException>(() => jornada.Cancel());

            Assert.Contains("cancelada", exception.Message.ToLower());
        }

        [Fact]
        public void Jornada_Finalize_Success()
        {
            var jornada = new JornadaReforestacionDataBuilder().Build();
            jornada.Initialize("REF-2026-001", DateTime.UtcNow.AddDays(14), 10,
                new List<DetalleArbolEntity> { new DetalleArbolJornadaDataBuilder().WithQuantity(10).Build() });

            jornada.Finalizar();

            Assert.Equal(JornadaStateEnum.Finalizada, jornada.State);
        }

        #region Name Validation

        [Fact]
        public void Jornada_Name_WithEmptyString_ThrowsException()
        {
            // Arrange
            var jornada = new JornadaReforestacionDataBuilder().Build();

            // Act & Assert
            var exception = Assert.Throws<RequiredException>(() => jornada.Name = "");
            Assert.Contains("no puede estar vacío", exception.Message);
        }

        [Fact]
        public void Jornada_Name_WithNull_ThrowsException()
        {
            // Arrange
            var jornada = new JornadaReforestacionDataBuilder().Build();

            // Act & Assert
            var exception = Assert.Throws<RequiredException>(() => jornada.Name = null!);
            Assert.Contains("no puede estar vacío", exception.Message);
        }

        [Fact]
        public void Jornada_Name_WithLessThan3Characters_ThrowsException()
        {
            // Arrange
            var jornada = new JornadaReforestacionDataBuilder().Build();

            // Act & Assert
            var exception = Assert.Throws<RequiredException>(() => jornada.Name = "AB");
            Assert.Contains("entre 3 y 200", exception.Message);
        }

        [Fact]
        public void Jornada_Name_WithMoreThan200Characters_ThrowsException()
        {
            // Arrange
            var jornada = new JornadaReforestacionDataBuilder().Build();
            var longName = new string('A', 201);

            // Act & Assert
            var exception = Assert.Throws<RequiredException>(() => jornada.Name = longName);
            Assert.Contains("entre 3 y 200", exception.Message);
        }

        [Fact]
        public void Jornada_Name_WithValidData_Success()
        {
            // Arrange
            var jornada = new JornadaReforestacionDataBuilder().Build();

            // Act
            jornada.Name = "Jornada Reforestación Parque Central";

            // Assert
            Assert.Equal("Jornada Reforestación Parque Central", jornada.Name);
        }

        [Fact]
        public void Jornada_Name_WithExactly3Characters_Success()
        {
            // Arrange
            var jornada = new JornadaReforestacionDataBuilder().Build();

            // Act
            jornada.Name = "ABC";

            // Assert
            Assert.Equal("ABC", jornada.Name);
        }

        [Fact]
        public void Jornada_Name_WithExactly200Characters_Success()
        {
            // Arrange
            var jornada = new JornadaReforestacionDataBuilder().Build();
            var name200 = new string('A', 200);

            // Act
            jornada.Name = name200;

            // Assert
            Assert.Equal(name200, jornada.Name);
        }

        #endregion

        #region Zona Validation

        [Fact]
        public void Jornada_Zona_WithNull_ThrowsException()
        {
            // Arrange
            var jornada = new JornadaReforestacionDataBuilder().Build();

            // Act & Assert
            var exception = Assert.Throws<RequiredException>(() => jornada.Zona = null!);
            Assert.Contains("no puede ser nula", exception.Message);
        }

        #endregion

        #region VolunteerCapacity Validation

        [Fact]
        public void Jornada_VolunteerCapacity_WithZero_ThrowsException()
        {
            // Arrange
            var jornada = new JornadaReforestacionDataBuilder().Build();

            // Act & Assert
            var exception = Assert.Throws<RequiredException>(() => jornada.VolunteerCapacity = 0);
            Assert.Contains("mayor a cero", exception.Message);
        }

        [Fact]
        public void Jornada_VolunteerCapacity_WithNegativeValue_ThrowsException()
        {
            // Arrange
            var jornada = new JornadaReforestacionDataBuilder().Build();

            // Act & Assert
            var exception = Assert.Throws<RequiredException>(() => jornada.VolunteerCapacity = -5);
            Assert.Contains("mayor a cero", exception.Message);
        }

        [Fact]
        public void Jornada_VolunteerCapacity_WithValidData_Success()
        {
            // Arrange
            var jornada = new JornadaReforestacionDataBuilder().Build();

            // Act
            jornada.VolunteerCapacity = 20;

            // Assert
            Assert.Equal(20, jornada.VolunteerCapacity);
        }

        #endregion

        #region TotalInscritos Validation

        [Fact]
        public void Jornada_TotalInscritos_WithNegativeValue_ThrowsException()
        {
            // Arrange
            var jornada = new JornadaReforestacionDataBuilder().Build();

            // Act & Assert
            var exception = Assert.Throws<CoreBusinessException>(() => jornada.TotalInscritos = -1);
            Assert.Contains("mayor o igual a cero", exception.Message);
        }

        [Fact]
        public void Jornada_TotalInscritos_WithZero_Success()
        {
            // Arrange
            var jornada = new JornadaReforestacionDataBuilder().Build();

            // Act
            jornada.TotalInscritos = 0;

            // Assert
            Assert.Equal(0, jornada.TotalInscritos);
        }

        [Fact]
        public void Jornada_TotalInscritos_WithPositiveValue_Success()
        {
            // Arrange
            var jornada = new JornadaReforestacionDataBuilder().Build();

            // Act
            jornada.TotalInscritos = 15;

            // Assert
            Assert.Equal(15, jornada.TotalInscritos);
        }

        #endregion

        #region DetalleArboles Validation

        [Fact]
        public void Jornada_DetalleArboles_WithNull_ThrowsException()
        {
            // Arrange
            var jornada = new JornadaReforestacionDataBuilder().Build();

            // Act & Assert
            var exception = Assert.Throws<RequiredException>(() => jornada.DetalleArboles = null!);
            Assert.Contains("no pueden ser nulos", exception.Message);
        }

        [Fact]
        public void Jornada_DetalleArboles_WithEmptyList_ThrowsException()
        {
            // Arrange
            var jornada = new JornadaReforestacionDataBuilder().Build();

            // Act & Assert
            var exception = Assert.Throws<RequiredException>(() => jornada.DetalleArboles = new List<DetalleArbolEntity>());
            Assert.Contains("no pueden estar vacíos", exception.Message);
        }

        [Fact]
        public void Jornada_DetalleArboles_WithValidData_Success()
        {
            // Arrange
            var jornada = new JornadaReforestacionDataBuilder().Build();
            var detalles = new List<DetalleArbolEntity>
            {
                new DetalleArbolJornadaDataBuilder().WithQuantity(5).Build(),
                new DetalleArbolJornadaDataBuilder().WithQuantity(5).Build()
            };

            // Act
            jornada.DetalleArboles = detalles;

            // Assert
            Assert.Equal(2, jornada.DetalleArboles.Count);
        }

        #endregion

        #region IsFull Property

        [Fact]
        public void Jornada_IsFull_WhenInscritosEqualsCapacity_ReturnsTrue()
        {
            // Arrange
            var jornada = new JornadaReforestacionDataBuilder()
                .WithVolunteerCapacity(10)
                .Build();
            jornada.TotalInscritos = 10;

            // Act & Assert
            Assert.True(jornada.IsFull);
        }

        [Fact]
        public void Jornada_IsFull_WhenInscritosExceedsCapacity_ReturnsTrue()
        {
            // Arrange
            var jornada = new JornadaReforestacionDataBuilder()
                .WithVolunteerCapacity(10)
                .Build();
            jornada.TotalInscritos = 15;

            // Act & Assert
            Assert.True(jornada.IsFull);
        }

        [Fact]
        public void Jornada_IsFull_WhenInscritosLessThanCapacity_ReturnsFalse()
        {
            // Arrange
            var jornada = new JornadaReforestacionDataBuilder()
                .WithVolunteerCapacity(10)
                .Build();
            jornada.TotalInscritos = 5;

            // Act & Assert
            Assert.False(jornada.IsFull);
        }

        [Fact]
        public void Jornada_IsFull_WhenNoInscritos_ReturnsFalse()
        {
            // Arrange
            var jornada = new JornadaReforestacionDataBuilder()
                .WithVolunteerCapacity(10)
                .Build();
            jornada.TotalInscritos = 0;

            // Act & Assert
            Assert.False(jornada.IsFull);
        }

        #endregion

        #region TotalAssignedTrees Property

        [Fact]
        public void Jornada_TotalAssignedTrees_CalculatesCorrectly()
        {
            // Arrange
            var jornada = new JornadaReforestacionDataBuilder().Build();
            var detalles = new List<DetalleArbolEntity>
            {
                new DetalleArbolJornadaDataBuilder().WithQuantity(5).Build(),
                new DetalleArbolJornadaDataBuilder().WithQuantity(3).Build(),
                new DetalleArbolJornadaDataBuilder().WithQuantity(2).Build()
            };
            jornada.DetalleArboles = detalles;

            // Act & Assert
            Assert.Equal(10, jornada.TotalAssignedTrees);
        }

        #endregion

        #region Factory Method Create

        [Fact]
        public void Jornada_Create_WithValidData_ReturnsInitializedJornada()
        {
            // Arrange
            var zona = new ZonaUrbanaDataBuilder().Build();
            var especie1 = new EspecieArboreaEntity 
            { 
                Id = Guid.NewGuid(),
                Name = "Pino", 
                ScientificName = "Pinus", 
                MaxHeightMeters = 20m 
            };
            var especie2 = new EspecieArboreaEntity 
            { 
                Id = Guid.NewGuid(),
                Name = "Encino", 
                ScientificName = "Quercus", 
                MaxHeightMeters = 15m 
            };
            var detalle1 = new DetalleArbolEntity
            {
                Especie = especie1,
                EspecieArboreaId = especie1.Id,
                Quantity = 6
            };
            var detalle2 = new DetalleArbolEntity
            {
                Especie = especie2,
                EspecieArboreaId = especie2.Id,
                Quantity = 4
            };

            // Act
            var jornada = JornadaReforestacion.Create(
                "Jornada Test",
                zona,
                DateTime.UtcNow.AddDays(14),
                10,
                2,
                new List<DetalleArbolEntity> { detalle1, detalle2 });

            // Assert
            Assert.NotNull(jornada);
            Assert.Equal("Jornada Test", jornada.Name);
            Assert.Equal(JornadaStateEnum.ConvocatoriaAbierta, jornada.State);
            Assert.NotNull(jornada.CodigoUnico);
            Assert.Equal(10, jornada.TotalAssignedTrees);
        }

        #endregion

        #region Cancel Additional Tests

        [Fact]
        public void Jornada_Cancel_WhenFinalizada_ThrowsException()
        {
            // Arrange
            var jornada = new JornadaReforestacionDataBuilder().Build();
            jornada.Initialize("REF-2026-001", DateTime.UtcNow.AddDays(14), 10,
                new List<DetalleArbolEntity> { new DetalleArbolJornadaDataBuilder().WithQuantity(10).Build() });
            jornada.Finalizar();

            // Act & Assert
            var exception = Assert.Throws<CoreBusinessException>(() => jornada.Cancel());
            Assert.Contains("finalizada", exception.Message.ToLower());
        }

        #endregion

        #region Initialize Additional Tests

        [Fact]
        public void Jornada_Initialize_SetsStateToConvocatoriaAbierta()
        {
            // Arrange
            var jornada = new JornadaReforestacionDataBuilder().Build();

            // Act
            jornada.Initialize("REF-2026-001", DateTime.UtcNow.AddDays(14), 10,
                new List<DetalleArbolEntity> { new DetalleArbolJornadaDataBuilder().WithQuantity(10).Build() });

            // Assert
            Assert.Equal(JornadaStateEnum.ConvocatoriaAbierta, jornada.State);
        }

        [Fact]
        public void Jornada_Initialize_SetsCodigoUnico()
        {
            // Arrange
            var jornada = new JornadaReforestacionDataBuilder().Build();

            // Act
            jornada.Initialize("REF-2026-001", DateTime.UtcNow.AddDays(14), 10,
                new List<DetalleArbolEntity> { new DetalleArbolJornadaDataBuilder().WithQuantity(10).Build() });

            // Assert
            Assert.Equal("REF-2026-001", jornada.CodigoUnico);
        }

        #endregion
    }
}
