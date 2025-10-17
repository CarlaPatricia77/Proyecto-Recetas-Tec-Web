using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recetas.Core.Entities;

namespace Recetas.Infrastructure.Data.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.HasKey(u => u.Id).HasName("PK_Usuario");

            builder.ToTable("Usuarios");

            builder.Property(u => u.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Correo)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(u => u.Correo)
                .IsUnique();

            builder.Property(u => u.Contrasena)
    .HasMaxLength(100)
    .IsRequired(false); // 👈 cambia a false

            builder.Property(r => r.FechaCreacion)
    .HasColumnType("datetime(6)")
    .IsRequired();

            builder.Property(u => u.IsActive)
                .IsRequired();
        }
    }
}
