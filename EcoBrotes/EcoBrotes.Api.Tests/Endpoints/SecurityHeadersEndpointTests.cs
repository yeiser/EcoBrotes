using System.Net;
using System.Net.Http.Json;

namespace EcoBrotes.Api.Tests.Endpoints;

// Verifica que el SecurityHeadersMiddleware agregue los encabezados de seguridad
// a las respuestas (exito y error), recorriendo el pipeline HTTP real.
public class SecurityHeadersEndpointTests
{
    private static readonly string[] ExpectedHeaders =
    {
        "X-Content-Type-Options",
        "X-XSS-Protection",
        "Content-Security-Policy",
        "Strict-Transport-Security",
        "Referrer-Policy",
        "Permissions-Policy"
    };

    [Fact]
    public async Task Get_Response_IncludesAllSecurityHeaders()
    {
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();

        var response = await client.GetAsync("/api/zonas");

        foreach (var header in ExpectedHeaders)
        {
            Assert.True(response.Headers.Contains(header), $"Falta el encabezado de seguridad '{header}'.");
        }
    }

    [Fact]
    public async Task SecurityHeaders_HaveExpectedValues()
    {
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();

        var response = await client.GetAsync("/api/zonas");

        Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").Single());
        Assert.Equal("default-src 'self'", response.Headers.GetValues("Content-Security-Policy").Single());
        Assert.Contains("max-age=31536000", response.Headers.GetValues("Strict-Transport-Security").Single());
    }

    [Fact]
    public async Task ErrorResponse_AlsoIncludesSecurityHeaders()
    {
        await using var webApp = new ApiApp();
        var client = webApp.CreateClient();

        // POST invalido -> 400 (validacion). Los headers deben estar igual.
        var response = await client.PostAsJsonAsync("/api/zonas", new { name = "AB" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(response.Headers.Contains("X-Content-Type-Options"));
        Assert.True(response.Headers.Contains("Content-Security-Policy"));
    }
}
