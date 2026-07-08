using EcoBrotes.Application.Zonas.Command;
using EcoBrotes.Application.Zonas.Query;
using EcoBrotes.Application.Jornadas.Query;
using EcoBrotes.Api.Filters;
using EcoBrotes.Domain.Exceptions;
using MediatR;

namespace EcoBrotes.Api.ApiHandlers;

public static class ZonasApi
{
    public static RouteGroupBuilder MapZonas(this IEndpointRouteBuilder routeHandler)
    {
        // POST - Create new urban zone
        routeHandler.MapPost("/", async (IMediator mediator, [Validate] CreateZonaCommand zona) =>
        {
            var zonaId = await mediator.Send(zona);
            return Results.Created($"/api/zonas/{zonaId}", zonaId);
        })
        .Produces(StatusCodes.Status201Created)
        .WithSummary("Create a new urban zone available for reforestation journeys")
        .WithOpenApi();

        // GET - Get zone by ID
        routeHandler.MapGet("/{id:guid}", async (IMediator mediator, Guid id) =>
        {
            var query = new GetZonaByIdQuery { Id = id };
            var zona = await mediator.Send(query);
            return Results.Ok(zona);
        })
        .Produces(StatusCodes.Status200OK)
        .WithSummary("Get an urban zone by ID")
        .WithOpenApi();

        // GET - List all urban zones
        routeHandler.MapGet("/", async (IMediator mediator) =>
        {
            var query = new GetZonasQuery();
            var zonas = await mediator.Send(query);
            return Results.Ok(zonas);
        })
        .Produces(StatusCodes.Status200OK)
        .WithSummary("List all urban zones available for reforestation journeys")
        .WithOpenApi();

        // PUT - Update urban zone
        routeHandler.MapPut("/{id:guid}", async (
            IMediator mediator,
            Guid id,
            [Validate] UpdateZonaCommand command) =>
        {
            if (id != command.Id)
            {
                throw new CoreBusinessException("El ID de la zona en la URL no coincide con el ID del comando.");
            }

            await mediator.Send(command);
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .WithSummary("Update an urban zone")
        .WithOpenApi();

        // DELETE - Delete urban zone
        routeHandler.MapDelete("/{id:guid}", async (
            IMediator mediator,
            Guid id) =>
        {
            var command = new DeleteZonaCommand(id);
            await mediator.Send(command);
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .WithSummary("Delete an urban zone")
        .WithOpenApi();

        return (RouteGroupBuilder)routeHandler;
    }
}
