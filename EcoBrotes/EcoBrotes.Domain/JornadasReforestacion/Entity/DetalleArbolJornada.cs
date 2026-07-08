using EcoBrotes.Domain.Common;
using EspecieArboreaEntity = EcoBrotes.Domain.EspecieArborea.Entity.EspecieArboreaEntity;

namespace EcoBrotes.Domain.JornadasReforestacion.Entity
{
    public class DetalleArbolJornada : DomainEntity
    {
        private EspecieArboreaEntity _especie = default!;
        private int _quantity;

        public Guid JornadaReforestacionId { get; set; }
        public Guid EspecieArboreaId { get; set; }

        public required EspecieArboreaEntity Especie
        {
            get => _especie;
            set
            {
                value.ValidateNull("la especie del árbol no puede ser nula.");
                _especie = value;
            }
        }

        public int Quantity
        {
            get => _quantity;
            set
            {
                value.ValidateGreaterThanZero("la cantidad debe ser mayor a cero.");
                _quantity = value;
            }
        }
    }
}
