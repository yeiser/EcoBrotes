namespace EcoBrotes.Application.Jornadas.Command
{
    /// <summary>
    /// Common interface for Jornada commands (Create and Update).
    /// Enables shared validation rules via FluentValidation.
    /// </summary>
    public interface IJornadaCommand
    {
        string Name { get; }
        DateTime ScheduledDate { get; }
        int TreeMeta { get; }
        int VolunteerCapacity { get; }
        IEnumerable<DetalleEspecieCommand> DetalleEspecies { get; }
    }
}
