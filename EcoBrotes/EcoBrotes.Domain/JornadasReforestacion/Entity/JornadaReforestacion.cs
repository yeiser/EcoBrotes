using EcoBrotes.Domain.Common;
using EcoBrotes.Domain.Exceptions;
using ZonaUrbanaEntity = EcoBrotes.Domain.ZonaUrbana.Entity.ZonaUrbanaEntity;
using DetalleArbolEntity = EcoBrotes.Domain.JornadasReforestacion.Entity.DetalleArbolJornada;

namespace EcoBrotes.Domain.JornadasReforestacion.Entity
{
    public class JornadaReforestacion : DomainEntity
    {
        const int MinLengthName = 3;
        const int MaxLengthName = 200;
        const int MinDaysAnticipation = 7;
        const int MinVolunteersPerTree = 5;

        private string _name = default!;
        private ZonaUrbanaEntity _zona = default!;
        private int _volunteerCapacity;
        private int _totalInscritos;
        private ICollection<DetalleArbolEntity> _detalleArboles = default!;

        public required string Name
        {
            get => _name;
            set
            {
                value.ValidateRequired("el nombre de la jornada no puede estar vacío o ser nulo.");
                value.ValidateLength(MinLengthName, MaxLengthName, $"el nombre de la jornada debe tener entre {MinLengthName} y {MaxLengthName} caracteres.");
                _name = value;
            }
        }

        public required ZonaUrbanaEntity Zona
        {
            get => _zona;
            set
            {
                value.ValidateNull("la zona urbana no puede ser nula.");
                _zona = value;
            }
        }

        public DateTime ScheduledDate { get; set; }

        public int TreeMeta { get; set; }

        public int VolunteerCapacity
        {
            get => _volunteerCapacity;
            set
            {
                value.ValidateGreaterThanZero("la capacidad de voluntarios debe ser mayor a cero.");
                _volunteerCapacity = value;
            }
        }

        public int TotalInscritos
        {
            get => _totalInscritos;
            set
            {
                if (value < 0)
                {
                    throw new CoreBusinessException("el total de inscritos debe ser mayor o igual a cero.");
                }
                _totalInscritos = value;
            }
        }

        public JornadaState State { get; set; }

        public required ICollection<DetalleArbolEntity> DetalleArboles
        {
            get => _detalleArboles;
            set
            {
                value.ValidateNull("los detalles de árboles no pueden ser nulos.");
                value.ValidateNotEmpty("los detalles de árboles no pueden estar vacíos.");
                _detalleArboles = value;
            }
        }

        public string CodigoUnico { get; set; } = string.Empty;

        public Guid ZonaUrbanaId { get; set; }

        public int TotalAssignedTrees => DetalleArboles.Sum(d => d.Quantity);

        public bool IsFull => VolunteerCapacity > 0 && TotalInscritos >= VolunteerCapacity;

        

        public static JornadaReforestacion Create(string nombre, ZonaUrbanaEntity zona, DateTime scheduledDate, int treeMeta, int volunteerCapacity, ICollection<DetalleArbolEntity> detalleArboles)
        {
            var jornada = new JornadaReforestacion
            {
                Name = nombre,
                Zona = zona,
                ZonaUrbanaId = zona.Id,
                VolunteerCapacity = volunteerCapacity,
                DetalleArboles = detalleArboles
            };
            jornada.Initialize(Guid.NewGuid().ToString("N"), scheduledDate, treeMeta, detalleArboles);
            return jornada;
        }

        public void Initialize(string codigoUnico, DateTime scheduledDate, int treeMeta, ICollection<DetalleArbolEntity> detalleArboles, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var minDate = now.AddDays(MinDaysAnticipation);

            if (scheduledDate < minDate)
            {
                throw new CoreBusinessException("Las jornadas deben programarse con al menos 7 d�as de anticipaci�n.");
            }

            _zona.Id.ValidateNotEmpty("el id de la zona es requerido.");
            ScheduledDate = scheduledDate;
            TreeMeta = treeMeta;
            CodigoUnico = codigoUnico;
            _detalleArboles = detalleArboles;
            State = JornadaState.ConvocatoriaAbierta;

            ValidateConsistency();
        }

        public void ValidateConsistency()
        {
            // RB-01: Consistencia de metas � sumatoria especies = meta total
            var totalSpecies = DetalleArboles.Sum(d => d.Quantity);
            if (totalSpecies != TreeMeta)
            {
                var difference = Math.Abs(totalSpecies - TreeMeta);
                throw new CoreBusinessException($"La suma de especies no coincide con la meta declarada. Diferencia: {difference} �rboles");
            }

            // RB-02: Proporci�n voluntario/�rbol
            var minVolunteers = (int)Math.Ceiling((double)TreeMeta / MinVolunteersPerTree);
            if (VolunteerCapacity < minVolunteers)
            {
                throw new CoreBusinessException($"El cupo m�nimo de voluntarios para esta jornada es {minVolunteers}");
            }

            // RB-04: Especie �nica � no duplicados
            var speciesIds = DetalleArboles.Select(d => d.EspecieArboreaId).ToList();
            var duplicates = speciesIds.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (duplicates.Any())
            {
                var duplicateSpecies = DetalleArboles.First(d => duplicates.Contains(d.EspecieArboreaId)).Especie.Name;
                throw new CoreBusinessException($"La especie {duplicateSpecies} ya ha sido agregada. Por favor, consolide las cantidades en una sola l�nea");
            }
        }

        public void Cancel()
        {
            if (State == JornadaState.Cancelada)
            {
                throw new CoreBusinessException("La jornada ya est� cancelada.");
            }
            if (State == JornadaState.Finalizada)
            {
                throw new CoreBusinessException("No se puede cancelar una jornada finalizada.");
            }
            State = JornadaState.Cancelada;
        }

        public void Finalizar()
        {
            if (State == JornadaState.Cancelada)
            {
                throw new CoreBusinessException("No se puede finalizar una jornada cancelada.");
            }
            State = JornadaState.Finalizada;
        }
    }
}


