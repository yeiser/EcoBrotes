using EcoBrotes.Domain.ZonaUrbana.Entity;

namespace EcoBrotes.Domain.Tests.ZonaUrbana.Entity
{
    public class ZonaUrbanaDataBuilder
    {
        Guid _id = Guid.NewGuid();
        string _name = "Zona de Prueba Norte";
        ZonaState _state = ZonaState.Activa;

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

        public ZonaUrbanaDataBuilder WithState(ZonaState state)
        {
            _state = state;
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
