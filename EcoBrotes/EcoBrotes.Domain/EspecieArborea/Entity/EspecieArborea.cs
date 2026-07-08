using EcoBrotes.Domain.Common;

namespace EcoBrotes.Domain.EspecieArborea.Entity
{
    public class EspecieArboreaEntity : DomainEntity
    {
        const int MinLengthName = 3;
        const int MaxLengthName = 150;

        private string _name = default!;
        private string _scientificName = default!;
        private decimal _maxHeightMeters;
        private EspecieState _state = EspecieState.Activa;

        public required string Name
        {
            get => _name;
            set
            {
                value.ValidateRequired("el nombre de la especie no puede estar vacío o ser nulo.");
                value.ValidateLength(MinLengthName, MaxLengthName, $"el nombre de la especie debe tener entre {MinLengthName} y {MaxLengthName} caracteres.");
                _name = value;
            }
        }

        public required string ScientificName
        {
            get => _scientificName;
            set
            {
                value.ValidateRequired("el nombre científico no puede estar vacío o ser nulo.");
                _scientificName = value;
            }
        }

        public decimal MaxHeightMeters
        {
            get => _maxHeightMeters;
            set
            {
                value.ValidateGreaterThanZero("la altura máxima debe ser mayor a cero.");
                _maxHeightMeters = value;
            }
        }

        public EspecieState State
        {
            get => _state;
            private set
            {
                value.ValidateNull("el estado no puede ser nulo.");
                _state = value;
            }
        }

        /// <summary>
        /// Deactivates the species (Soft Delete).
        /// This preserves referential integrity with related records.
        /// </summary>
        public void Deactivate()
        {
            State = EspecieState.Inactiva;
        }
    }
}
