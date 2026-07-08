using EcoBrotes.Domain.Exceptions;

namespace EcoBrotes.Application.Common
{
    /// <summary>
    /// Shared service for mapping domain entities to DTOs.
    /// Centralizes null validation and common mapping patterns.
    /// </summary>
    public static class EntityMapper
    {
        /// <summary>
        /// Validates entity exists and maps it to a DTO using the provided mapper function.
        /// </summary>
        /// <typeparam name="TEntity">The domain entity type</typeparam>
        /// <typeparam name="TDto">The DTO type</typeparam>
        /// <param name="entity">The entity to map</param>
        /// <param name="entityName">Display name for error messages</param>
        /// <param name="mapper">Function to perform the actual mapping</param>
        /// <returns>The mapped DTO</returns>
        /// <exception cref="CoreBusinessException">Thrown if entity is null</exception>
        public static TDto MapEntity<TEntity, TDto>(
            TEntity entity,
            string entityName,
            Func<TEntity, TDto> mapper) where TEntity : class
        {
            if (entity == null)
            {
                throw new CoreBusinessException($"{entityName} no existe.");
            }

            return mapper(entity);
        }

        /// <summary>
        /// Maps a collection of entities to DTOs.
        /// </summary>
        /// <typeparam name="TEntity">The domain entity type</typeparam>
        /// <typeparam name="TDto">The DTO type</typeparam>
        /// <param name="entities">Collection of entities to map</param>
        /// <param name="mapper">Function to perform the actual mapping</param>
        /// <returns>List of mapped DTOs</returns>
        public static List<TDto> MapEntities<TEntity, TDto>(
            IEnumerable<TEntity> entities,
            Func<TEntity, TDto> mapper)
        {
            return entities.Select(mapper).ToList();
        }
    }
}
