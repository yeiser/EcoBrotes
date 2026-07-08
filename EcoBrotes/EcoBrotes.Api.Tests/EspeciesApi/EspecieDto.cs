namespace EcoBrotes.Api.Tests.EspeciesApi
{
    public class EspecieDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ScientificName { get; set; } = string.Empty;
        public decimal MaxHeightMeters { get; set; }
    }
}
