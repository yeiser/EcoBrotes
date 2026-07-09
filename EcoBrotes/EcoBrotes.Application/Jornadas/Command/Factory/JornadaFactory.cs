using EcoBrotes.Application.Jornadas.Command;
using EcoBrotes.Domain.Common;
using EcoBrotes.Domain.EspecieArborea.Entity;
using EcoBrotes.Domain.EspecieArborea.Port;
using EcoBrotes.Domain.Exceptions;
using EcoBrotes.Domain.JornadasReforestacion.Entity;
using EcoBrotes.Domain.ZonaUrbana.Entity;
using EcoBrotes.Domain.ZonaUrbana.Port;

namespace EcoBrotes.Application.Jornadas.Command.Factory
{
    public class JornadaFactory(
        IZonaUrbanaRepository zonaRepository,
        IEspecieArboreaRepository especieRepository)
    {
        public async Task<JornadaReforestacion> CreateAsync(
            CreateJornadaCommand command,
            string codigoUnico)
        {
            var zona = await zonaRepository.GetByIdAsync(command.ZonaUrbanaId);
            zona.ValidateNull($"La zona urbana con id {command.ZonaUrbanaId} no existe.");

            // RB-06: Zone must be active to create a jornada
            if (zona.State == ZonaState.Inactiva)
            {
                throw new CoreBusinessException($"La zona urbana con id {command.ZonaUrbanaId} está desactivada y no puede ser usada para crear jornadas.");
            }

            var detalleArboles = await ValidateEspeciesAsync(command.DetalleEspecies, "crear");

            var jornada = new JornadaReforestacion
            {
                Name = command.Name,
                Zona = zona,
                ZonaUrbanaId = zona.Id,
                VolunteerCapacity = command.VolunteerCapacity,
                DetalleArboles = detalleArboles,
                CodigoUnico = codigoUnico
            };

            jornada.Initialize(codigoUnico, command.ScheduledDate, command.TreeMeta, detalleArboles);

            return jornada;
        }

        /// <summary>
        /// Validates that zone and species exist, and maps species to DetalleArbolJornada.
        /// This moves cross-cutting validation logic out of the handler.
        /// </summary>
        public async Task<List<DetalleArbolJornada>> ValidateUpdateAsync(
            Guid zonaId,
            IEnumerable<DetalleEspecieCommand> detalleEspecies)
        {
            // Validate zone exists and is active
            var zona = await zonaRepository.GetByIdAsync(zonaId);
            zona.ValidateNull($"La zona urbana con id {zonaId} no existe.");

            // RB-06: Zone must be active to update a jornada
            if (zona.State == ZonaState.Inactiva)
            {
                throw new CoreBusinessException($"La zona urbana con id {zonaId} está desactivada y no puede ser usada para actualizar jornadas.");
            }

            return await ValidateEspeciesAsync(detalleEspecies, "actualizar");
        }

        /// <summary>
        /// Validates all species in the command and maps them to DetalleArbolJornada entities.
        /// This method is reused by both CreateAsync and ValidateUpdateAsync to avoid code duplication.
        /// </summary>
        /// <param name="detalleEspecies">List of species details to validate</param>
        /// <param name="operation">Operation name for error messages ("crear" or "actualizar")</param>
        /// <returns>List of validated DetalleArbolJornada entities</returns>
        private async Task<List<DetalleArbolJornada>> ValidateEspeciesAsync(
            IEnumerable<DetalleEspecieCommand> detalleEspecies,
            string operation)
        {
            var detalleArboles = new List<DetalleArbolJornada>();

            foreach (var detalleEspecie in detalleEspecies)
            {
                var especie = await especieRepository.GetByIdAsync(detalleEspecie.EspecieArboreaId);
                especie.ValidateNull($"La especie con id {detalleEspecie.EspecieArboreaId} no existe.");

                // RB-06: Species must be active to be used in a jornada
                if (especie.State == EspecieState.Inactiva)
                {
                    throw new CoreBusinessException(
                        $"La especie con id {detalleEspecie.EspecieArboreaId} está desactivada y no puede ser usada para {operation} jornadas.");
                }

                // RB-03: Each especie must have at least 1 tree
                if (detalleEspecie.Quantity < 1)
                {
                    throw new CoreBusinessException(
                        $"La especie con id {detalleEspecie.EspecieArboreaId} debe tener al menos 1 árbol asignado. Cantidad actual: {detalleEspecie.Quantity}");
                }

                detalleArboles.Add(new DetalleArbolJornada
                {
                    Especie = especie,
                    EspecieArboreaId = especie.Id,
                    Quantity = detalleEspecie.Quantity
                });
            }

            return detalleArboles;
        }
    }
}
