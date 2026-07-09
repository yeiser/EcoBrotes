using EcoBrotes.Domain.Exceptions;
using JornadaEntity = EcoBrotes.Domain.JornadasReforestacion.Entity.JornadaReforestacion;
using DetalleArbolEntity = EcoBrotes.Domain.JornadasReforestacion.Entity.DetalleArbolJornada;
using ZonaUrbanaEntity = EcoBrotes.Domain.ZonaUrbana.Entity.ZonaUrbanaEntity;
using EspecieArboreaEntity = EcoBrotes.Domain.EspecieArborea.Entity.EspecieArboreaEntity;

namespace EcoBrotes.Domain.Tests.JornadasReforestacion.Entity
{
    public class JornadaReforestacionDataBuilder
    {
        Guid _id;
        string _name = "Jornada Reforestación Zona Norte";
        DateTime _scheduledDate = DateTime.UtcNow.AddDays(14);
        int _treeMeta = 10;
        int _volunteerCapacity = 3;
        string _codigoUnico = "REF-2026-001";
        List<DetalleArbolEntity>? _detalleArboles;
        readonly string _especieName = "Especie Test";
        readonly ZonaUrbanaEntity _zona = new() { Id = Guid.NewGuid(), Name = "Zona de Prueba" };
        
        private List<DetalleArbolEntity> CreateDefaultDetalleArboles()
        {
            return new()
            {
                new DetalleArbolEntity
                {
                    Especie = new EspecieArboreaEntity { Id = Guid.NewGuid(), Name = _especieName, ScientificName = "Spec test", MaxHeightMeters = 10m },
                    Quantity = 10
                }
            };
        }

        public JornadaReforestacionDataBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public JornadaReforestacionDataBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public JornadaReforestacionDataBuilder WithScheduledDate(DateTime date)
        {
            _scheduledDate = date;
            return this;
        }

        public JornadaReforestacionDataBuilder WithTreeMeta(int meta)
        {
            _treeMeta = meta;
            return this;
        }

        public JornadaReforestacionDataBuilder WithVolunteerCapacity(int capacity)
        {
            _volunteerCapacity = capacity;
            return this;
        }

        public JornadaReforestacionDataBuilder WithDetalleArboles(List<DetalleArbolEntity> detalle)
        {
            _detalleArboles = detalle;
            return this;
        }

        public JornadaReforestacionDataBuilder WithCodigoUnico(string codigo)
        {
            _codigoUnico = codigo;
            return this;
        }

        public JornadaEntity Build()
        {
            if (_detalleArboles == null)
            {
                _detalleArboles = CreateDefaultDetalleArboles();
            }
            return new JornadaEntity
            {
                Id = _id,
                Name = _name,
                Zona = _zona,
                ScheduledDate = _scheduledDate,
                TreeMeta = _treeMeta,
                VolunteerCapacity = _volunteerCapacity,
                CodigoUnico = _codigoUnico,
                DetalleArboles = _detalleArboles
            };
        }
    }
}
