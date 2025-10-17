using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Recetas.Core.Entities;

namespace Recetas.Infrastructure.Data
{
    public partial class RecetasContext : DbContext
    {
        public RecetasContext()
        {
        }

        public RecetasContext(DbContextOptions<RecetasContext> options)
            : base(options)
        {
        }

        // DbSets (Tablas del sistema)
        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<Receta> Recetas { get; set; }
        public virtual DbSet<Categoria> Categorias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aplica automáticamente todas las configuraciones (Receta, Usuario, Categoria)
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
