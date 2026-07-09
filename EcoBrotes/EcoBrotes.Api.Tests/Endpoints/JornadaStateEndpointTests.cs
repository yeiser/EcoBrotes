using System.Net;
using System.Net.Http.Json;

namespace EcoBrotes.Api.Tests.Endpoints;

// Integracion HTTP de las reglas de transicion de estado de una jornada:
// solo se puede editar en ConvocatoriaAbierta y no se puede cancelar dos veces.
public class JornadaStateEndpointTests
{
    private static async Task<Guid> SeedJornada(HttpClient client)
    {
        var zonaResp = await client.PostAsJsonAsync("/api/zonas", new { name = "Zona Norte" });
        var zonaId = await zonaResp.Content.ReadFromJsonAsync<Guid>();

        var especieResp = await client.PostAsJsonAsync("/api/especies",
            new { name = "Roble", scientificName = "Quercus robur", maxHeightMeters = 20m });
        var especieId = await especieResp.Content.ReadFromJsonAsync<Guid>();

        var jornadaResp = await client.PostAsJsonAsync("/api/jornadas", new
        {
            zonaUrbanaId = zonaId,
            name = "Jornada estado",
            scheduledDate = DateTime.UtcNow.AddDays(14),
            treeMeta = 10,
            volunteerCapacity = 2,
            detalleEspecies = new[] { new { especieArboreaId = especieId, quantity = 10 } }
        });
        return await jornadaResp.Content.ReadFromJsonAsync<Guid>();
    }

    private static object UpdateBody(Guid id) => new
    {
        id,
        zonaUrbanaId = Guid.NewGuid(),
        name = "Jornada actualizada",
        scheduledDate = DateTime.UtcNow.AddDays(20),
        treeMeta = 10,
        volunteerCapacity = 2,
        detalleEspecies = new[] { new { especieArboreaId = Guid.NewGuid(), quantity = 10 } }
    };

    [Fact]
    public async Task Cancel_AlreadyCancelledJornada_Returns400()
    {
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();
        var id = await SeedJornada(client);

        var first = await client.DeleteAsync($"/api/jornadas/{id}");
        Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);

        // Segunda cancelacion: la entidad rechaza cancelar una jornada ya cancelada.
        var second = await client.DeleteAsync($"/api/jornadas/{id}");
        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
    }

    [Fact]
    public async Task Put_CancelledJornada_Returns400()
    {
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();
        var id = await SeedJornada(client);

        await client.DeleteAsync($"/api/jornadas/{id}"); // cancelar

        // Solo se pueden editar jornadas en estado ConvocatoriaAbierta.
        var response = await client.PutAsJsonAsync($"/api/jornadas/{id}", UpdateBody(id));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Cancel_NonExistentJornada_Returns400()
    {
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();

        var response = await client.DeleteAsync($"/api/jornadas/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_NonExistentJornada_Returns400()
    {
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();

        var response = await client.GetAsync($"/api/jornadas/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
