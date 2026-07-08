using EspecieArboreaEntity = EcoBrotes.Domain.EspecieArborea.Entity.EspecieArboreaEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoBrotes.Infrastructure.DataSource.ModelConfig
{
    internal class EspecieArboreaConfig : IEntityTypeConfiguration<EspecieArboreaEntity>
    {
        const int MaxLengthName = 150;

        public void Configure(EntityTypeBuilder<EspecieArboreaEntity> builder)
        {
            builder.Property(e => e.Id)
                .IsRequired();

            builder.Property(e => e.Name)
                .HasMaxLength(MaxLengthName)
                .IsRequired();

            builder.Property(e => e.ScientificName)
                .IsRequired();

            builder.Property(e => e.MaxHeightMeters)
                .HasColumnType("decimal(18,2)");
        }
    }
}
