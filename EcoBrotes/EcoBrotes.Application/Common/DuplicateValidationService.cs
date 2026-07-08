using EcoBrotes.Domain.Exceptions;

namespace EcoBrotes.Application.Common
{
    /// <summary>
    /// Shared service for validating duplicate names across entities.
    /// Prevents creating entities with duplicate names in their respective repositories.
    /// </summary>
    public static class DuplicateValidationService
    {
        /// <summary>
        /// Validates that no existing entity has the same name.
        /// </summary>
        /// <typeparam name="T">The entity type being validated</typeparam>
        /// <param name="getByNameAsync">Function to retrieve entity by name</param>
        /// <param name="entityName">Display name of the entity type (e.g., "especie", "zona urbana")</param>
        /// <param name="name">The name to validate</param>
        /// <exception cref="CoreBusinessException">Thrown if an entity with the same name already exists</exception>
        public static async Task ValidateNoDuplicateNameAsync<T>(
            Func<Task<T?>> getByNameAsync,
            string entityName,
            string name) where T : class
        {
            var existing = await getByNameAsync();
            if (existing != null)
            {
                throw new CoreBusinessException($"Ya existe {entityName} con el nombre '{name}'.");
            }
        }
    }
}
