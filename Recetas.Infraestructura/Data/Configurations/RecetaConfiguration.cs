using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recetas.Core.Entities;

namespace Recetas.Infrastructure.Data.Configurations
{
    public class RecetaConfiguration : IEntityTypeConfiguration<Receta>
    {
        public void Configure(EntityTypeBuilder<Receta> builder)
        {
            builder.HasKey(r => r.Id).HasName("PK_Receta");

            builder.ToTable("Recetas");

            builder.Property(r => r.Titulo)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.Descripcion)
                .HasMaxLength(500);

            builder.Property(r => r.Ingredientes)
                .IsRequired();

            builder.Property(r => r.TiempoPreparacion)
                .IsRequired();

            builder.Property(r => r.FechaCreacion)
    .HasColumnType("datetime(6)")
    .IsRequired();

            builder.Property(r => r.IsActive)
                .IsRequired();

            // Relaciones: Receta pertenece a un Usuario
            builder.HasOne(r => r.Usuario)
                .WithMany(u => u.Recetas)
                .HasForeignKey(r => r.UsuarioId)
                .HasConstraintName("FK_Receta_Usuario")
                .OnDelete(DeleteBehavior.SetNull);

            // Relaciones: Receta pertenece a una Categoria
            builder.HasOne(r => r.Categoria)
                .WithMany(c => c.Recetas)
                .HasForeignKey(r => r.CategoriaId)
                .HasConstraintName("FK_Receta_Categoria")
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
