using EcoBrotes.Application.Jornadas.Command;

namespace EcoBrotes.Api.Tests.JornadaApi;

public class CreateJornadaCommandBuilder
{
    Guid _zonaUrbanaId;
    string _name = "Jornada Reforestación Norte";
    DateTime _scheduledDate = DateTime.UtcNow.AddDays(14);
    int _treeMeta = 10;
    int _volunteerCapacity = 2;
    IEnumerable<DetalleEspecieCommand> _detalleEspecies;

    public CreateJornadaCommand Build() => new(_zonaUrbanaId, _name, _scheduledDate, _treeMeta, _volunteerCapacity, _detalleEspecies);

    public CreateJornadaCommandBuilder WithZonaUrbanaId(Guid zonaUrbanaId) { _zonaUrbanaId = zonaUrbanaId; return this; }
    public CreateJornadaCommandBuilder WithName(string name) { _name = name; return this; }
    public CreateJornadaCommandBuilder WithScheduledDate(DateTime date) { _scheduledDate = date; return this; }
    public CreateJornadaCommandBuilder WithTreeMeta(int meta) { _treeMeta = meta; return this; }
    public CreateJornadaCommandBuilder WithVolunteerCapacity(int capacity) { _volunteerCapacity = capacity; return this; }
    public CreateJornadaCommandBuilder WithDetalleEspecies(IEnumerable<DetalleEspecieCommand> detalle) { _detalleEspecies = detalle; return this; }
}
