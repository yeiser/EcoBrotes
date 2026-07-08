using EcoBrotes.Domain.EspecieArborea.Entity;
using EcoBrotes.Domain.JornadasReforestacion.Entity;
using EcoBrotes.Domain.ZonaUrbana.Entity;

namespace EcoBrotes.Application.Tests.Common
{
    /// <summary>
    /// Minimal factory to build valid JornadaReforestacion instances with a given state
    /// for referential-integrity scenarios in the Application layer tests.
    /// </summary>
    internal static class JornadaBuilder
    {
        public static JornadaReforestacion WithState(JornadaState state, string codigoUnico)
        {
            var jornada = new JornadaReforestacion
            {
                Name = "Jornada de prueba",
                Zona = new ZonaUrbanaEntity { Id = Guid.NewGuid(), Name = "Zona Norte" },
                VolunteerCapacity = 5,
                DetalleArboles = new List<DetalleArbolJornada>
                {
                    new()
                    {
                        Especie = new EspecieArboreaEntity { Name = "Roble", ScientificName = "Quercus", MaxHeightMeters = 20m },
                        Quantity = 10
                    }
                },
                CodigoUnico = codigoUnico,
                State = state
            };
            return jornada;
        }
    }
}
