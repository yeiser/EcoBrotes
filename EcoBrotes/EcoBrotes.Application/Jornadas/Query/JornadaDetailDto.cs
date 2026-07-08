namespace EcoBrotes.Application.Jornadas.Query
{
    public class JornadaDetailDto
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
        public IEnumerable<JornadaEspecieDetailDto> Especies { get; set; } = new List<JornadaEspecieDetailDto>();
    }

    public class JornadaEspecieDetailDto
    {
        public Guid EspecieId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string NombreCientifico { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal AlturaMaxMetros { get; set; }
    }
}
