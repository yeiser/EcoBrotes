using ZonaUrbanaEntity = EcoBrotes.Domain.ZonaUrbana.Entity.ZonaUrbanaEntity;

namespace EcoBrotes.Api.Tests.JornadaApi
{
    public class ZonaUrbanaDataBuilder
    {
        Guid _id;
        string _name = "Zona Norte";

        public ZonaUrbanaDataBuilder WithId(Guid id) { _id = id; return this; }
        public ZonaUrbanaDataBuilder WithName(string name) { _name = name; return this; }

        public ZonaUrbanaEntity Build() => new() { Id = _id == Guid.Empty ? Guid.NewGuid() : _id, Name = _name };
    }
}
