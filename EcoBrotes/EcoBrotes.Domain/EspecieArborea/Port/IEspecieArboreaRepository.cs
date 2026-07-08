using EspecieArboreaEntity = EcoBrotes.Domain.EspecieArborea.Entity.EspecieArboreaEntity;

namespace EcoBrotes.Domain.EspecieArborea.Port
{
    public interface IEspecieArboreaRepository
    {
        Task<EspecieArboreaEntity> AddAsync(EspecieArboreaEntity especie);
        Task<EspecieArboreaEntity> GetByIdAsync(Guid id);
        Task<EspecieArboreaEntity> GetByIdAsync(Guid id, string? include);
        Task<EspecieArboreaEntity> GetByNameAsync(string name);
        Task<int> GetCountAsync();
        Task<IEnumerable<EspecieArboreaEntity>> GetManyAsync();
        Task DeactivateAsync(Guid id);
    }
}
