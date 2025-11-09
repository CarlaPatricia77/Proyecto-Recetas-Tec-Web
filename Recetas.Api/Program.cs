using Microsoft.EntityFrameworkCore;
using Recetas.Infrastructure.Data;
using Recetas.Core.Interfaces;
using Recetas.Infrastructure.Repositories;
using Recetas.Infrastructure.Filters;
using Recetas.Infrastructure.Validators;
using Recetas.Infrastructure.Mappings;
using Recetas.Core.Services;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

#region Configurar la BD MySql
var connectionString = builder.Configuration.GetConnectionString("ConnectionMySql");
builder.Services.AddDbContext<RecetasContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
#endregion

// =====================================
// CONFIGURACIÓN DE SERVICIOS PRINCIPALES
// =====================================

// Controladores + JSON + desactivar filtro default + GlobalExceptionFilter
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
    options.Filters.Add<GlobalExceptionFilter>(); // ? Nuevo filtro global
})
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling =
        Newtonsoft.Json.ReferenceLoopHandling.Ignore;
})
.ConfigureApiBehaviorOptions(options =>
{
    // Desactiva la validación automática de ModelState
    options.SuppressModelStateInvalidFilter = true;
});

// =====================================
// PATRÓN UNIT OF WORK
// =====================================
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// =====================================
// REPOSITORIOS (Base Repository Genérico)
// =====================================
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

// =====================================
// SERVICES (Lógica de Negocio)
// =====================================
builder.Services.AddScoped<IRecetaService, RecetaService>();

// =====================================
// VALIDADORES (FluentValidation)
// =====================================
// Descomentar cuando crees los validadores:
// builder.Services.AddValidatorsFromAssemblyContaining<RecetaValidator>();
// builder.Services.AddValidatorsFromAssemblyContaining<UsuarioValidator>();
// builder.Services.AddValidatorsFromAssemblyContaining<CategoriaValidator>();

// Servicio de validación
builder.Services.AddScoped<IValidationService, ValidationService>();

// =====================================
// AUTOMAPPER
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