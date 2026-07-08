using DetalleArbolEntity = EcoBrotes.Domain.JornadasReforestacion.Entity.DetalleArbolJornada;
using EspecieArboreaEntity = EcoBrotes.Domain.EspecieArborea.Entity.EspecieArboreaEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoBrotes.Infrastructure.DataSource.ModelConfig
{
    internal class DetalleArbolJornadaConfig : IEntityTypeConfiguration<DetalleArbolEntity>
    {
        public void Configure(EntityTypeBuilder<DetalleArbolEntity> builder)
        {
            builder.Property(d => d.Id)
                .IsRequired();

            builder.Property(d => d.Quantity)
                .IsRequired();

            builder.HasOne(d => d.Especie)
                .WithMany()
                .HasForeignKey(d => d.EspecieArboreaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
