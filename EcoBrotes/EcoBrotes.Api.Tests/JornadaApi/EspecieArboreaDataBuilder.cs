using EspecieArboreaEntity = EcoBrotes.Domain.EspecieArborea.Entity.EspecieArboreaEntity;

namespace EcoBrotes.Api.Tests.JornadaApi
{
    public class EspecieArboreaDataBuilder
    {
        Guid _id;
        string _name = "Pino";
        string _scientificName = "Pinus";
        decimal _maxHeightMeters = 20m;

        public EspecieArboreaDataBuilder WithId(Guid id) { _id = id; return this; }
        public EspecieArboreaDataBuilder WithName(string name) { _name = name; return this; }
        public EspecieArboreaDataBuilder WithScientificName(string sciName) { _scientificName = sciName; return this; }
        public EspecieArboreaDataBuilder WithMaxHeightMeters(decimal height) { _maxHeightMeters = height; return this; }

        public EspecieArboreaEntity Build() => new() { Id = _id == Guid.Empty ? Guid.NewGuid() : _id, Name = _name, ScientificName = _scientificName, MaxHeightMeters = _maxHeightMeters };
    }
}
