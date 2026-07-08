using ZonaUrbanaEntity = EcoBrotes.Domain.ZonaUrbana.Entity.ZonaUrbanaEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoBrotes.Infrastructure.DataSource.ModelConfig
{
    internal class ZonaUrbanaConfig : IEntityTypeConfiguration<ZonaUrbanaEntity>
    {
        const int MaxLengthName = 100;

        public void Configure(EntityTypeBuilder<ZonaUrbanaEntity> builder)
        {
            builder.Property(z => z.Id)
                .IsRequired();

            builder.Property(z => z.Name)
                .HasMaxLength(MaxLengthName)
                .IsRequired();
        }
    }
}
