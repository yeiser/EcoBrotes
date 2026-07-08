namespace EcoBrotes.Application.Jornadas.Query
{
    public class PaginatedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => TotalCount > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
    }
}
