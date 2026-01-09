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
// CONFIGURACIÓN DE CONTROLADORES Y FILTROS
// =====================================
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
    options.Filters.Add<ValidationFilter>();
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
// UNIT OF WORK
// =====================================
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// =====================================
// REPOSITORIOS GENÉRICOS
// =====================================
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

// =====================================
// SERVICES DE NEGOCIO
// =====================================
builder.Services.AddScoped<IRecetaService, RecetaService>();

// =====================================
// VALIDACIÓN (FluentValidation + Servicio propio)
// =====================================
builder.Services.AddValidatorsFromAssemblyContaining<RecetaValidator>();
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

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// =====================================
// PIPELINE DE MIDDLEWARE
// =====================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Recetas v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

// (Opcional para futuro)
// app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.Run();
