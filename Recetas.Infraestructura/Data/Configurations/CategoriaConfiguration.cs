using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recetas.Core.Entities;

namespace Recetas.Infrastructure.Data.Configurations
{
    public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
    {
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            builder.HasKey(c => c.Id).HasName("PK_Categoria");

            builder.ToTable("Categorias");

            builder.Property(c => c.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(c => c.Nombre)
                .IsUnique();

            builder.Property(c => c.Descripcion)
                .HasMaxLength(255);

            builder.Property(r => r.FechaCreacion)
    .HasColumnType("datetime(6)")
    .IsRequired();

            builder.Property(c => c.IsActive)
                .IsRequired();
        }
    }
}
