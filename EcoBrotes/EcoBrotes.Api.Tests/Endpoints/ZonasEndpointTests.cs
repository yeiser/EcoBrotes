using System.Net;
using System.Net.Http.Json;

namespace EcoBrotes.Api.Tests.Endpoints;

// Pruebas de integracion HTTP sobre el controlador de Zonas.
public class ZonasEndpointTests
{
    [Fact]
    public async Task Post_ValidZona_Returns201AndIsRetrievable()
    {
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();

        var response = await client.PostAsJsonAsync("/api/zonas", new { name = "Zona Norte" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var id = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, id);

        var get = await client.GetAsync($"/api/zonas/{id}");
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
    }

    [Fact]
    public async Task Post_InvalidZona_Returns400()
    {
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();

        // Nombre demasiado corto: viola CreateZonaCommandValidator (Length 3..100).
        var response = await client.PostAsJsonAsync("/api/zonas", new { name = "AB" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_DuplicateName_Returns400()
    {
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();

        await client.PostAsJsonAsync("/api/zonas", new { name = "Zona Duplicada" });
        var response = await client.PostAsJsonAsync("/api/zonas", new { name = "Zona Duplicada" });

        // Regla de negocio (nombre duplicado) -> CoreBusinessException -> 400.
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_ValidZona_Returns204()
    {
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();

        var created = await client.PostAsJsonAsync("/api/zonas", new { name = "Zona Norte" });
        var id = await created.Content.ReadFromJsonAsync<Guid>();

        var response = await client.PutAsJsonAsync($"/api/zonas/{id}", new { id, name = "Zona Sur" });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingZona_Returns204()
    {
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();

        var created = await client.PostAsJsonAsync("/api/zonas", new { name = "Zona Norte" });
        var id = await created.Content.ReadFromJsonAsync<Guid>();

        var response = await client.DeleteAsync($"/api/zonas/{id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
