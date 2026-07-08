namespace EcoBrotes.Application.Jornadas.Query
{
    public class JornadaSummaryDto
    {
        public Guid Id { get; set; }
        public string CodigoUnico { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ZonaNombre { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public int TreeMeta { get; set; }
        public int VolunteerCapacity { get; set; }
        public int TotalInscritos { get; set; }
        public decimal OcupacionPct { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
