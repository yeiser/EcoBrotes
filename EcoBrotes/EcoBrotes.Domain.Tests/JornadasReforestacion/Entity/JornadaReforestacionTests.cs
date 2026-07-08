using EcoBrotes.Domain.Exceptions;
using EspecieArboreaEntity = EcoBrotes.Domain.EspecieArborea.Entity.EspecieArboreaEntity;
using DetalleArbolEntity = EcoBrotes.Domain.JornadasReforestacion.Entity.DetalleArbolJornada;
using JornadaEntity = EcoBrotes.Domain.JornadasReforestacion.Entity.JornadaReforestacion;
using JornadaStateEnum = EcoBrotes.Domain.JornadasReforestacion.Entity.JornadaState;

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
    }
}
