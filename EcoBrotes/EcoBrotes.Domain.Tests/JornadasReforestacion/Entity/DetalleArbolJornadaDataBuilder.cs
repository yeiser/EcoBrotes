using EcoBrotes.Domain.Exceptions;
using EspecieArboreaEntity = EcoBrotes.Domain.EspecieArborea.Entity.EspecieArboreaEntity;
using DetalleArbolEntity = EcoBrotes.Domain.JornadasReforestacion.Entity.DetalleArbolJornada;

namespace EcoBrotes.Domain.Tests.JornadasReforestacion.Entity
{
    public class DetalleArbolJornadaDataBuilder
    {
        Guid _id;
        EspecieArboreaEntity _especie = new() { Id = Guid.NewGuid(), Name = "Test", ScientificName = "Test", MaxHeightMeters = 10m };
        int _quantity = 5;
        Guid _jornadaReforestacionId;
        Guid _especieArboreaId;

        public DetalleArbolJornadaDataBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public DetalleArbolJornadaDataBuilder WithEspecie(EspecieArboreaEntity especie)
        {
            _especie = especie;
            return this;
        }

        public DetalleArbolJornadaDataBuilder WithQuantity(int quantity)
        {
            _quantity = quantity;
            return this;
        }

        public DetalleArbolJornadaDataBuilder WithJornadaReforestacionId(Guid jornadaId)
        {
            _jornadaReforestacionId = jornadaId;
            return this;
        }

        public DetalleArbolJornadaDataBuilder WithEspecieArboreaId(Guid especieId)
        {
            _especieArboreaId = especieId;
            return this;
        }

        public DetalleArbolEntity Build()
        {
            return new DetalleArbolEntity
            {
                Id = _id,
                Especie = _especie,
                Quantity = _quantity,
                JornadaReforestacionId = _jornadaReforestacionId,
                EspecieArboreaId = _especie.Id
            };
        }
    }
}
