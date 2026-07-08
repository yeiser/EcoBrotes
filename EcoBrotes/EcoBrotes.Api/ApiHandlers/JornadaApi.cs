using EcoBrotes.Application.Jornadas.Command;
using EcoBrotes.Application.Jornadas.Query;
using EcoBrotes.Api.Filters;
using EcoBrotes.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace EcoBrotes.Api.ApiHandlers;

public static class JornadaApi
{
    public static RouteGroupBuilder MapJornada(this IEndpointRouteBuilder routeHandler)
    {
        // POST - Create new journey
        routeHandler.MapPost("/", async (IMediator mediator, [Validate] CreateJornadaCommand jornada) =>
        {
            var jornadaId = await mediator.Send(jornada);
            return Results.Created($"/api/jornadas/{jornadaId}", jornadaId);
        })
        .Produces(StatusCodes.Status201Created)
        .WithSummary("Create new reforestation journey")
        .WithOpenApi();

        // GET - Get journey by ID
        routeHandler.MapGet("/{id:guid}", async (IMediator mediator, Guid id) =>
        {
            var query = new GetJornadaByIdQuery { Id = id };
            var jornada = await mediator.Send(query);
            return Results.Ok(jornada);
        })
        .Produces(StatusCodes.Status200OK)
        .WithSummary("Get a reforestation journey by ID")
        .WithOpenApi();

        // GET - Search and filter journeys
        routeHandler.MapGet("/", async (
            IMediator mediator,
            IValidator<GetJornadasRequest> validator,
            HttpContext context,
            Guid? zonaId,
            string? estados,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            int page,
            int pageSize) =>
        {
            // Create request object for validation
            var request = new GetJornadasRequest
            {
                ZonaId = zonaId,
                Estados = estados,
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta,
                Page = page,
                PageSize = pageSize
            };

            // Validate request
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary(),
                    statusCode: (int)System.Net.HttpStatusCode.BadRequest);
            }

            // Parse states
            IEnumerable<string>? estadoList = null;
            if (!string.IsNullOrEmpty(estados))
            {
                estadoList = estados.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            }

            var query = new GetJornadasQuery
            {
                ZonaId = request.ZonaId,
                Estados = estadoList,
                FechaDesde = request.FechaDesde,
                FechaHasta = request.FechaHasta,
                Page = request.Page,
                PageSize = request.PageSize
            };

            var result = await mediator.Send(query);
            
            // Add pagination headers
            context.Response.Headers.Append("X-Total-Count", result.TotalCount.ToString());
            context.Response.Headers.Append("X-Page", result.Page.ToString());
            context.Response.Headers.Append("X-Page-Size", result.PageSize.ToString());
            context.Response.Headers.Append("X-Total-Pages", result.TotalPages.ToString());
            
            // Add Link header for next/prev pages
            var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}";
            var linkHeader = new List<string>();
            
            if (result.Page > 1)
            {
                var prevUrl = $"{baseUrl}?page={result.Page - 1}&pageSize={result.PageSize}";
                linkHeader.Add($"<{prevUrl}>; rel=\"prev\"");
            }
            
            if (result.Page < result.TotalPages)
            {
                var nextUrl = $"{baseUrl}?page={result.Page + 1}&pageSize={result.PageSize}";
                linkHeader.Add($"<{nextUrl}>; rel=\"next\"");
            }
            
            if (linkHeader.Count > 0)
            {
                context.Response.Headers.Append("Link", string.Join(", ", linkHeader));
            }

            return Results.Ok(result.Items);
        })
        .Produces(StatusCodes.Status200OK)
        .WithSummary("Search and filter reforestation journeys")
        .WithOpenApi();

        // PUT - Update journey (only ConvocatoriaAbierta)
        routeHandler.MapPut("/{id:guid}", async (
            IMediator mediator,
            Guid id,
            [Validate] UpdateJornadaCommand command) =>
        {
            if (id != command.Id)
            {
                throw new CoreBusinessException("El ID de la jornada en la URL no coincide con el ID del comando.");
            }

            await mediator.Send(command);
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .WithSummary("Update a reforestation journey (only allowed when state is ConvocatoriaAbierta)")
        .WithOpenApi();

        // DELETE - Cancel journey (only if not Finalizada)
        routeHandler.MapDelete("/{id:guid}", async (
            IMediator mediator,
            Guid id) =>
        {
            var command = new CancelJornadaCommand(id);
            await mediator.Send(command);
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .WithSummary("Cancel a reforestation journey (not allowed if state is Finalizada)")
        .WithOpenApi();

        return (RouteGroupBuilder)routeHandler;
    }
}
