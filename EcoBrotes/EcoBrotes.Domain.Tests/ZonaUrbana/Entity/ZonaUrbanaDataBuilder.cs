using EcoBrotes.Domain.ZonaUrbana.Entity;

namespace EcoBrotes.Domain.Tests.ZonaUrbana.Entity
{
    public class ZonaUrbanaDataBuilder
    {
        Guid _id = Guid.NewGuid();
        string _name = "Zona de Prueba Norte";

        public ZonaUrbanaDataBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public ZonaUrbanaDataBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public ZonaUrbanaEntity Build()
        {
            var zona = new ZonaUrbanaEntity
            {
                Id = _id,
                Name = _name
            };
            return zona;
        }
    }
}
