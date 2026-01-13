using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recetas.Core.Entities;

namespace Recetas.Infrastructure.Data.Configurations
{
    public class RecetaConfiguration : IEntityTypeConfiguration<Receta>
    {
        public void Configure(EntityTypeBuilder<Receta> builder)
        {
            builder.HasKey(r => r.Id).HasName("PK_recetas");
            builder.ToTable("recetas"); // minúscula

            builder.Property(r => r.Id)
                .HasColumnName("id"); // minúscula

            builder.Property(r => r.titulo)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.descripcion)
                .HasMaxLength(500);

            builder.Property(r => r.ingredientes)
                .IsRequired();

            builder.Property(r => r.tiempo_preparacion)
                .HasColumnName("tiempo_preparacion") // minúscula y guion bajo
                .IsRequired();

            builder.Property(r => r.fecha_creacion)
                .HasColumnName("fecha_creacion") // minúscula y guion bajo
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(r => r.IsActive)
                .HasColumnName("IsActive")
                .IsRequired();

            builder.Property(r => r.usuarioId)
                .HasColumnName("usuario_id"); // minúscula y guion bajo

            builder.Property(r => r.categoria_id)
                .HasColumnName("categoria_id"); // minúscula y guion bajo

            builder.HasOne(r => r.Usuario)
                .WithMany(u => u.Recetas)
                .HasForeignKey(r => r.usuarioId)
                .HasConstraintName("fk_usuario_receta")
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(r => r.Categoria)
                .WithMany(c => c.Recetas)
                .HasForeignKey(r => r.categoria_id)
                .HasConstraintName("fk_categoria_receta")
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
