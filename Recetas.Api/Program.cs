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
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
    options.Filters.Add<GlobalExceptionFilter>();
})
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling =
        Newtonsoft.Json.ReferenceLoopHandling.Ignore;
})
.ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// =====================================
// PATRÓN UNIT OF WORK
// =====================================
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// =====================================
// REPOSITORIOS
// =====================================
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

// =====================================
// SERVICES
// =====================================
builder.Services.AddScoped<IRecetaService, RecetaService>();

// =====================================
// VALIDADORES
// =====================================
builder.Services.AddValidatorsFromAssemblyContaining<RecetaValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UsuarioValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CategoriaValidator>();
builder.Services.AddScoped<IValidationService, ValidationService>();

// =====================================
// AUTOMAPPER
// =====================================
builder.Services.AddAutoMapper(typeof(MappingProfile));

// =====================================
// SWAGGER
// =====================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API de Recetas",
        Version = "v1",
        Description = "API para gestión de recetas, usuarios y categorías",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Equipo de Desarrollo",
            Email = "desarrollo@recetas.com"
        }
    });

    // Incluir comentarios XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// =====================================
// CONFIGURAR MIDDLEWARE
// =====================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Recetas v1");
        options.RoutePrefix = string.Empty; // Swagger en la raíz
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();