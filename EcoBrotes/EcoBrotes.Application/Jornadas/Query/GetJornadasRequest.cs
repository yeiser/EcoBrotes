namespace EcoBrotes.Application.Jornadas.Query
{
    public class GetJornadasRequest
    {
        public Guid? ZonaId { get; set; }
        public string? Estados { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
