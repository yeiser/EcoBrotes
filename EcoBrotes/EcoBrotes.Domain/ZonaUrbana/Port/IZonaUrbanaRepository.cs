using ZonaUrbanaEntity = EcoBrotes.Domain.ZonaUrbana.Entity.ZonaUrbanaEntity;

namespace EcoBrotes.Domain.ZonaUrbana.Port
{
    public interface IZonaUrbanaRepository
    {
        Task<ZonaUrbanaEntity> AddAsync(ZonaUrbanaEntity zona);
        Task<ZonaUrbanaEntity> GetByIdAsync(Guid id);
        Task<ZonaUrbanaEntity> GetByIdAsync(Guid id, string? include);
        Task<ZonaUrbanaEntity?> GetByNameAsync(string name);
        Task<int> GetCountAsync();
        Task<IEnumerable<ZonaUrbanaEntity>> GetManyAsync();
        Task DeactivateAsync(Guid id);
    }
}
