using EcoBrotes.Domain.Exceptions;
using EcoBrotes.Domain.JornadasReforestacion.Entity;
using EcoBrotes.Domain.JornadasReforestacion.Port;

namespace EcoBrotes.Application.Common
{
    /// <summary>
    /// Shared service for validating referential integrity before soft-delete operations.
    /// Prevents deactivating entities that are still referenced by active jornadas.
    /// </summary>
    public static class ReferentialIntegrityService
    {
        /// <summary>
        /// Validates that no active jornadas reference the entity.
        /// Active jornadas are those that are NOT Finalizada or Cancelada.
        /// </summary>
        /// <param name="getReferencedJornadas">Function to retrieve jornadas referencing the entity</param>
        /// <param name="entityName">Display name of the entity (e.g., "especie", "zona urbana")</param>
        /// <param name="entityId">ID of the entity being deleted</param>
        /// <exception cref="CoreBusinessException">Thrown if active jornadas reference this entity</exception>
        public static async Task ValidateNoActiveReferencesAsync(
            Func<Task<IEnumerable<JornadaReforestacion>>> getReferencedJornadas,
            string entityName,
            Guid entityId)
        {
            var jornadasWithReference = await getReferencedJornadas();
            var activeJornadas = jornadasWithReference
                .Where(j => j.State != JornadaState.Finalizada && j.State != JornadaState.Cancelada)
                .ToList();

            if (activeJornadas.Any())
            {
                var jornadaCodes = string.Join(", ", activeJornadas.Select(j => j.CodigoUnico));
                throw new CoreBusinessException(
                    $"No se puede desactivar el {entityName} porque está siendo utilizada en las siguientes jornadas activas: {jornadaCodes}. " +
                    "Por favor, cancele o finalice estas jornadas primero.");
            }
        }
    }
}
