using EcoBrotes.Application.Especies.Command;
using EcoBrotes.Application.Especies.Query;
using EcoBrotes.Application.Jornadas.Query;
using EcoBrotes.Api.Filters;
using EcoBrotes.Domain.Exceptions;
using MediatR;

namespace EcoBrotes.Api.ApiHandlers;

public static class EspeciesApi
{
    public static RouteGroupBuilder MapEspecies(this IEndpointRouteBuilder routeHandler)
    {
        // POST - Create new tree species
        routeHandler.MapPost("/", async (IMediator mediator, [Validate] CreateEspecieCommand especie) =>
        {
            var especieId = await mediator.Send(especie);
            return Results.Created($"/api/especies/{especieId}", especieId);
        })
        .Produces(StatusCodes.Status201Created)
        .WithSummary("Create a new tree species available for reforestation journeys")
        .WithOpenApi();

        // GET - Get species by ID
        routeHandler.MapGet("/{id:guid}", async (IMediator mediator, Guid id) =>
        {
            var query = new GetEspecieByIdQuery { Id = id };
            var especie = await mediator.Send(query);
            return Results.Ok(especie);
        })
        .Produces(StatusCodes.Status200OK)
        .WithSummary("Get a tree species by ID")
        .WithOpenApi();

        // GET - List all tree species
        routeHandler.MapGet("/", async (IMediator mediator) =>
        {
            var query = new GetEspeciesQuery();
            var especies = await mediator.Send(query);
            return Results.Ok(especies);
        })
        .Produces(StatusCodes.Status200OK)
        .WithSummary("List all tree species available for reforestation journeys")
        .WithOpenApi();

        // PUT - Update tree species
        routeHandler.MapPut("/{id:guid}", async (
            IMediator mediator,
            Guid id,
            [Validate] UpdateEspecieCommand command) =>
        {
            if (id != command.Id)
            {
                throw new CoreBusinessException("El ID de la especie en la URL no coincide con el ID del comando.");
            }

            await mediator.Send(command);
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .WithSummary("Update a tree species")
        .WithOpenApi();

        // DELETE - Delete tree species
        routeHandler.MapDelete("/{id:guid}", async (
            IMediator mediator,
            Guid id) =>
        {
            var command = new DeleteEspecieCommand(id);
            await mediator.Send(command);
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .WithSummary("Delete a tree species")
        .WithOpenApi();

        return (RouteGroupBuilder)routeHandler;
    }
}
