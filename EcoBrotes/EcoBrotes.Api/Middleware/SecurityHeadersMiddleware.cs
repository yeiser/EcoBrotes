using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace EcoBrotes.Api.Middleware;

/// <summary>
/// Middleware que añade encabezados de seguridad a todas las respuestas HTTP.
/// Resuelve vulnerabilidades detectadas por OWASP ZAP.
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Configurar cookies con HttpOnly y Secure
        context.Response.OnStarting(() =>
        {
            // X-Content-Type-Options: Previene sniffing de contenido
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

            // X-XSS-Protection: Habilita el filtro XSS del navegador
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

            // Content-Security-Policy: Restringe los orígenes de contenido
            context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'");

            // Strict-Transport-Security: Fuerza HTTPS (en producción)
            context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");

            // Referrer-Policy: Controla la información de referrer
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

            // Permissions-Policy: Restringe características del navegador
            context.Response.Headers.Append("Permissions-Policy", "geolocation=(), microphone=(), camera=()");

            // Remover X-Powered-By si existe
            context.Response.Headers.Remove("X-Powered-By");

            return Task.CompletedTask;
        });

        await _next(context);
    }
}

// Extension method para facilitar el uso del middleware
public static class SecurityHeadersMiddlewareExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SecurityHeadersMiddleware>();
    }
}
