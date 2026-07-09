using System.Net;
using System.Net.Http.Json;

namespace EcoBrotes.Api.Tests.Endpoints;

// Integracion HTTP de la regla RB-09: no se puede desactivar una zona o especie
// que este siendo referenciada por una jornada activa. Recorre el pipeline
// completo (endpoint -> handler -> ReferentialIntegrityService -> middleware).
public class ReferentialIntegrityEndpointTests
{
    private static async Task<(Guid zonaId, Guid especieId, Guid jornadaId)> SeedZonaEspecieJornada(HttpClient client)
    {
        var zonaResp = await client.PostAsJsonAsync("/api/zonas", new { name = "Zona Norte" });
        var zonaId = await zonaResp.Content.ReadFromJsonAsync<Guid>();

        var especieResp = await client.PostAsJsonAsync("/api/especies",
            new { name = "Roble", scientificName = "Quercus robur", maxHeightMeters = 20m });
        var especieId = await especieResp.Content.ReadFromJsonAsync<Guid>();

        var jornadaResp = await client.PostAsJsonAsync("/api/jornadas", new
        {
            zonaUrbanaId = zonaId,
            name = "Jornada activa",
            scheduledDate = DateTime.UtcNow.AddDays(14),
            treeMeta = 10,
            volunteerCapacity = 2,
            detalleEspecies = new[] { new { especieArboreaId = especieId, quantity = 10 } }
        });
        var jornadaId = await jornadaResp.Content.ReadFromJsonAsync<Guid>();

        return (zonaId, especieId, jornadaId);
    }

    [Trait("ReglaNegocio", "RB-09")]
    [Fact]
    public async Task Delete_Zona_WithActiveJornada_Returns400()
    {
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();
        var (zonaId, _, _) = await SeedZonaEspecieJornada(client);

        var response = await client.DeleteAsync($"/api/zonas/{zonaId}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Trait("ReglaNegocio", "RB-09")]
    [Fact]
    public async Task Delete_Especie_WithActiveJornada_Returns400()
    {
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();
        var (_, especieId, _) = await SeedZonaEspecieJornada(client);

        var response = await client.DeleteAsync($"/api/especies/{especieId}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Trait("ReglaNegocio", "RB-09")]
    [Fact]
    public async Task Delete_Zona_AfterCancellingJornada_Returns204()
    {
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();
        var (zonaId, _, jornadaId) = await SeedZonaEspecieJornada(client);

        // Al cancelar la jornada deja de ser una referencia activa.
        var cancel = await client.DeleteAsync($"/api/jornadas/{jornadaId}");
        Assert.Equal(HttpStatusCode.NoContent, cancel.StatusCode);

        var response = await client.DeleteAsync($"/api/zonas/{zonaId}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
