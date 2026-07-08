namespace EcoBrotes.Application.Jornadas.Command
{
    public record DetalleEspecieCommand(
        Guid EspecieArboreaId,
        int Quantity
    );
}
