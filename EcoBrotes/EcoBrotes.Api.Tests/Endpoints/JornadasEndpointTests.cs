using System.Net;
using System.Net.Http.Json;

namespace EcoBrotes.Api.Tests.Endpoints;

// Pruebas de integracion HTTP sobre el controlador de Jornadas.
public class JornadasEndpointTests
{
    private static async Task<(Guid zonaId, Guid especieId)> SeedZonaAndEspecie(HttpClient client)
    {
        var zonaResp = await client.PostAsJsonAsync("/api/zonas", new { name = "Zona Norte" });
        var zonaId = await zonaResp.Content.ReadFromJsonAsync<Guid>();

        var especieResp = await client.PostAsJsonAsync("/api/especies",
            new { name = "Roble", scientificName = "Quercus robur", maxHeightMeters = 20m });
        var especieId = await especieResp.Content.ReadFromJsonAsync<Guid>();

        return (zonaId, especieId);
    }

    private static object ValidJornada(Guid zonaId, Guid especieId) => new
    {
        zonaUrbanaId = zonaId,
        name = "Jornada de prueba",
        scheduledDate = DateTime.UtcNow.AddDays(14),
        treeMeta = 10,
        volunteerCapacity = 2,
        detalleEspecies = new[] { new { especieArboreaId = especieId, quantity = 10 } }
    };

    [Fact]
    public async Task Post_ValidJornada_Returns201()
    {
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();
        var (zonaId, especieId) = await SeedZonaAndEspecie(client);

        var response = await client.PostAsJsonAsync("/api/jornadas", ValidJornada(zonaId, especieId));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var id = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, id);
    }

    [Fact]
    public async Task Post_InvalidJornada_Returns400()
    {
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();

        // ZonaUrbanaId vacio: viola CreateJornadaCommandValidator (NotEmpty) -> ValidationFilter -> 400.
        var response = await client.PostAsJsonAsync("/api/jornadas", new
        {
            zonaUrbanaId = Guid.Empty,
            name = "Jornada invalida",
            scheduledDate = DateTime.UtcNow.AddDays(14),
            treeMeta = 10,
            volunteerCapacity = 2,
            detalleEspecies = new[] { new { especieArboreaId = Guid.NewGuid(), quantity = 10 } }
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_CancelsJornada_Returns204()
    {
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();
        var (zonaId, especieId) = await SeedZonaAndEspecie(client);

        var created = await client.PostAsJsonAsync("/api/jornadas", ValidJornada(zonaId, especieId));
        var id = await created.Content.ReadFromJsonAsync<Guid>();

        var response = await client.DeleteAsync($"/api/jornadas/{id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Get_CreatedJornada_ReturnsDetail()
    {
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();
        var (zonaId, especieId) = await SeedZonaAndEspecie(client);

        var created = await client.PostAsJsonAsync("/api/jornadas", ValidJornada(zonaId, especieId));
        var id = await created.Content.ReadFromJsonAsync<Guid>();

        var response = await client.GetAsync($"/api/jornadas/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
