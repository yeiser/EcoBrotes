using EcoBrotes.Api.ApiHandlers;
using EcoBrotes.Application.Especies.Command;
using EcoBrotes.Application.Zonas.Command;

namespace EcoBrotes.Api.Tests.Validators;

public class CommandValidatorsTests
{
    [Fact]
    public void CreateEspecie_Valid_Passes()
    {
        var result = new CreateEspecieCommandValidator()
            .Validate(new CreateEspecieCommand("Roble", "Quercus robur", 20m));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateEspecie_Invalid_Fails()
    {
        var result = new CreateEspecieCommandValidator()
            .Validate(new CreateEspecieCommand("", "", 0m));
        Assert.False(result.IsValid);
    }

    [Fact]
    public void UpdateEspecie_Valid_Passes()
    {
        var result = new UpdateEspecieCommandValidator()
            .Validate(new UpdateEspecieCommand(Guid.NewGuid(), "Ceiba", "Ceiba pentandra", 40m));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void UpdateEspecie_Invalid_Fails()
    {
        var result = new UpdateEspecieCommandValidator()
            .Validate(new UpdateEspecieCommand(Guid.Empty, "AB", "", -1m));
        Assert.False(result.IsValid);
    }

    [Fact]
    public void DeleteEspecie_Valid_Passes()
    {
        var result = new DeleteEspecieCommandValidator().Validate(new DeleteEspecieCommand(Guid.NewGuid()));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void DeleteEspecie_EmptyId_Fails()
    {
        var result = new DeleteEspecieCommandValidator().Validate(new DeleteEspecieCommand(Guid.Empty));
        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateZona_Valid_Passes()
    {
        var result = new CreateZonaCommandValidator().Validate(new CreateZonaCommand("Zona Norte"));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateZona_Invalid_Fails()
    {
        var result = new CreateZonaCommandValidator().Validate(new CreateZonaCommand("AB"));
        Assert.False(result.IsValid);
    }

    [Fact]
    public void UpdateZona_Valid_Passes()
    {
        var result = new UpdateZonaCommandValidator().Validate(new UpdateZonaCommand(Guid.NewGuid(), "Zona Sur"));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void UpdateZona_Invalid_Fails()
    {
        var result = new UpdateZonaCommandValidator().Validate(new UpdateZonaCommand(Guid.Empty, ""));
        Assert.False(result.IsValid);
    }

    [Fact]
    public void DeleteZona_Valid_Passes()
    {
        var result = new DeleteZonaCommandValidator().Validate(new DeleteZonaCommand(Guid.NewGuid()));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void DeleteZona_EmptyId_Fails()
    {
        var result = new DeleteZonaCommandValidator().Validate(new DeleteZonaCommand(Guid.Empty));
        Assert.False(result.IsValid);
    }
}
