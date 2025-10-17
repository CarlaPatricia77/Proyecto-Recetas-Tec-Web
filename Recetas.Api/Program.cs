using Microsoft.EntityFrameworkCore;
using Recetas.Infrastructure.Data;
using Recetas.Core.Interfaces;
using Recetas.Infrastructure.Repositories;
using Recetas.Infrastructure.Filters;
using Recetas.Infrastructure.Validators;
using FluentValidation;
using Recetas.Infrastructure.Mappings;


var builder = WebApplication.CreateBuilder(args);

#region Configurar la BD MySql
var connectionString = builder.Configuration.GetConnectionString("ConnectionMySql");
builder.Services.AddDbContext<RecetasContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
#endregion

// =====================================
// CONFIGURACIÓN DE SERVICIOS PRINCIPALES
// =====================================

// Controladores + JSON + desactivar filtro default
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling =
            Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        // Desactiva la validación automática de ModelState
        // para que funcione el ValidationFilter personalizado
        options.SuppressModelStateInvalidFilter = true;
    });

// =====================================
// REPOSITORIOS
// =====================================
builder.Services.AddTransient<IRecetasRepository, RecetaRepository>();
builder.Services.AddTransient<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddTransient<ICategoriasRepository, CategoriaRepository>();

// =====================================
// VALIDADORES (FluentValidation)
// =====================================
// builder.Services.AddValidatorsFromAssemblyContaining<RecetaValidator>();
// builder.Services.AddValidatorsFromAssemblyContaining<UsuarioValidator>();
// builder.Services.AddValidatorsFromAssemblyContaining<CategoriaValidator>();

// Servicio de validación
builder.Services.AddScoped<IValidationService, ValidationService>();

// =====================================
// FILTROS
// =====================================
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

// =====================================
// AUTOMAPPER (opcional)
// =====================================
builder.Services.AddAutoMapper(typeof(MappingProfile));

// =====================================
// CONSTRUIR Y EJECUTAR LA APLICACIÓN
// =====================================
var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
