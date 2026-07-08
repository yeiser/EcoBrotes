using EcoBrotes.Domain.Common;
using EcoBrotes.Domain.JornadasReforestacion.Port;

namespace EcoBrotes.Domain.JornadasReforestacion.Service
{
    [DomainService]
    public class GenerateCodigoUnicoService(IJornadaReforestacionRepository jornadaRepository)
    {
        public async Task<string> ExecuteAsync()
        {
            var year = DateTime.UtcNow.Year;
            var count = await jornadaRepository.GetCountAsync();
            var nextNumber = count + 1;
            return $"REF-{year}-{nextNumber:D3}";
        }
    }
}
