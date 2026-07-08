using JornadaEntity = EcoBrotes.Domain.JornadasReforestacion.Entity.JornadaReforestacion;
using DetalleArbolEntity = EcoBrotes.Domain.JornadasReforestacion.Entity.DetalleArbolJornada;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoBrotes.Infrastructure.DataSource.ModelConfig
{
    internal class JornadaReforestacionConfig : IEntityTypeConfiguration<JornadaEntity>
    {
        const int MaxLengthName = 200;

        public void Configure(EntityTypeBuilder<JornadaEntity> builder)
        {
            builder.Property(j => j.Id)
                .IsRequired();

            builder.Property(j => j.Name)
                .HasMaxLength(MaxLengthName)
                .IsRequired();

            builder.Property(j => j.ScheduledDate)
                .IsRequired();

            builder.Property(j => j.TreeMeta)
                .IsRequired();

            builder.Property(j => j.VolunteerCapacity)
                .IsRequired();

            builder.Property(j => j.TotalInscritos)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(j => j.State)
                .HasColumnType("smallint")
                .IsRequired();

            builder.Property(j => j.CodigoUnico)
                .IsRequired();

            builder.HasMany(j => j.DetalleArboles)
                .WithOne()
                .HasForeignKey(d => d.JornadaReforestacionId);
        }
    }
}
