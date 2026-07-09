using EcoBrotes.Domain.EspecieArborea.Entity;

namespace EcoBrotes.Domain.Tests.EspecieArborea.Entity
{
    public class EspecieArboreaDataBuilder
    {
        Guid _id = Guid.NewGuid();
        string _name = "Pino Nacional";
        string _scientificName = "Pinus pseudostrobus";
        decimal _maxHeightMeters = 25m;

        public EspecieArboreaDataBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public EspecieArboreaDataBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public EspecieArboreaDataBuilder WithScientificName(string scientificName)
        {
            _scientificName = scientificName;
            return this;
        }

        public EspecieArboreaDataBuilder WithMaxHeightMeters(decimal maxHeight)
        {
            _maxHeightMeters = maxHeight;
            return this;
        }

        public EspecieArboreaEntity Build()
        {
            var especie = new EspecieArboreaEntity
            {
                Id = _id,
                Name = _name,
                ScientificName = _scientificName,
                MaxHeightMeters = _maxHeightMeters
            };
            return especie;
        }
    }
}
