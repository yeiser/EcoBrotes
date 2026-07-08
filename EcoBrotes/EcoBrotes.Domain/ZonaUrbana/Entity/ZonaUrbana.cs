using EcoBrotes.Domain.Common;

namespace EcoBrotes.Domain.ZonaUrbana.Entity
{
    public class ZonaUrbanaEntity : DomainEntity
    {
        const int MinLengthName = 3;
        const int MaxLengthName = 100;

        private string _name = default!;
        private ZonaState _state = ZonaState.Activa;

        public required string Name
        {
            get => _name;
            set
            {
                value.ValidateRequired("el nombre de la zona no puede estar vacío o ser nulo.");
                value.ValidateLength(MinLengthName, MaxLengthName, $"el nombre de la zona debe tener entre {MinLengthName} y {MaxLengthName} caracteres.");
                _name = value;
            }
        }

        public ZonaState State
        {
            get => _state;
            private set
            {
                value.ValidateNull("el estado no puede ser nulo.");
                _state = value;
            }
        }

        /// <summary>
        /// Deactivates the zone (Soft Delete).
        /// This preserves referential integrity with related records.
        /// </summary>
        public void Deactivate()
        {
            State = ZonaState.Inactiva;
        }
    }
}
