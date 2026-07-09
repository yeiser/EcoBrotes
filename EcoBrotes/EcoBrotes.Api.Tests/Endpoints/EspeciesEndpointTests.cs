using System.Net;
using System.Net.Http.Json;

namespace EcoBrotes.Api.Tests.Endpoints;

// Pruebas de integracion HTTP sobre el controlador de Especies:
// ejercitan el pipeline completo (routing + ValidationFilter + middleware de excepciones).
public class EspeciesEndpointTests
{
    private static object ValidBody(string name = "Roble") =>
        new { name, scientificName = "Quercus robur", maxHeightMeters = 20m };

    [Fact]
    public async Task Post_ValidEspecie_Returns201AndIsRetrievable()
    {
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();

        var response = await client.PostAsJsonAsync("/api/especies", ValidBody());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var id = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, id);

        var get = await client.GetAsync($"/api/especies/{id}");
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
    }

    [Fact]
    public async Task Post_InvalidEspecie_Returns400()
    {
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();

        // Nombre vacio: viola CreateEspecieCommandValidator (NotEmpty + Length).
        var response = await client.PostAsJsonAsync("/api/especies",
            new { name = "", scientificName = "", maxHeightMeters = 0m });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_ValidEspecie_Returns204()
    {
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();

        var created = await client.PostAsJsonAsync("/api/especies", ValidBody());
        var id = await created.Content.ReadFromJsonAsync<Guid>();

        var response = await client.PutAsJsonAsync($"/api/especies/{id}",
            new { id, name = "Ceiba", scientificName = "Ceiba pentandra", maxHeightMeters = 40m });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Put_IdMismatch_Returns400()
    {
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();

        var created = await client.PostAsJsonAsync("/api/especies", ValidBody());
        var id = await created.Content.ReadFromJsonAsync<Guid>();

        // El id de la URL no coincide con el del cuerpo -> CoreBusinessException -> 400.
        var response = await client.PutAsJsonAsync($"/api/especies/{id}",
            new { id = Guid.NewGuid(), name = "Ceiba", scientificName = "Ceiba pentandra", maxHeightMeters = 40m });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingEspecie_Returns204()
    {
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();

        var created = await client.PostAsJsonAsync("/api/especies", ValidBody());
        var id = await created.Content.ReadFromJsonAsync<Guid>();

        var response = await client.DeleteAsync($"/api/especies/{id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
