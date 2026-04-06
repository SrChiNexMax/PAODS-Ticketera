using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PdsTk.Aplicacion.Abstracciones.Persistencia;
using PdsTk.Aplicacion.Abstracciones.Seguridad;
using PdsTk.Aplicacion.CasosDeUso.Autenticacion;
using PdsTk.Aplicacion.CasosDeUso.Incidentes;
using PdsTk.Aplicacion.CasosDeUso.PoliticasSla;
using PdsTk.Aplicacion.CasosDeUso.Usuarios;
using PdsTk.Aplicacion.Servicios;
using PdsTk.Infraestructura.Persistencia;
using PdsTk.Infraestructura.Persistencia.Inicializadores;
using PdsTk.Infraestructura.Persistencia.Repositorios;
using PdsTk.Infraestructura.Seguridad;

var builder = WebApplication.CreateBuilder(args);

var jwtOptions = new JwtOptions
{
    Issuer = builder.Configuration["Jwt:Issuer"] ?? "PdsTk.Api",
    Audience = builder.Configuration["Jwt:Audience"] ?? "PdsTk.Frontend",
    SecretKey = builder.Configuration["Jwt:SecretKey"] ?? "PdsTk-Dev-Secret-Key-2026-Change-This-Please",
    ExpiracionMinutos = int.TryParse(builder.Configuration["Jwt:ExpiracionMinutos"], out var expiracionMinutos)
        ? expiracionMinutos
        : 120
};

var corsOrigins = builder.Configuration
    .GetSection("Cors:Origins")
    .GetChildren()
    .Select(x => x.Value)
    .Where(x => !string.IsNullOrWhiteSpace(x))
    .Cast<string>()
    .ToArray();

if (corsOrigins.Length == 0)
{
    corsOrigins =
    [
        "http://localhost:5173",
        "http://127.0.0.1:5173"
    ];
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PdsTk API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<PdsTkDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSqlServer")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSingleton(jwtOptions);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IIncidenteRepository, IncidenteRepository>();
builder.Services.AddScoped<IPoliticaSlaRepository, PoliticaSlaRepository>();
builder.Services.AddScoped<IComentarioIncidenteRepository, ComentarioIncidenteRepository>();
builder.Services.AddScoped<IRegistroAuditoriaRepository, RegistroAuditoriaRepository>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddSingleton<ITokenJwtService, JwtTokenService>();

builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();
builder.Services.AddScoped<ISlaService, SlaService>();
builder.Services.AddSingleton<ICodigoIncidenteService, CodigoIncidenteService>();

builder.Services.AddScoped<IniciarSesionCasoUso>();
builder.Services.AddScoped<ObtenerPerfilActualCasoUso>();
builder.Services.AddScoped<ListarAgentesCasoUso>();
builder.Services.AddScoped<ListarPoliticasSlaCasoUso>();
builder.Services.AddScoped<CrearIncidenteCasoUso>();
builder.Services.AddScoped<ListarIncidentesCasoUso>();
builder.Services.AddScoped<ObtenerDetalleIncidenteCasoUso>();
builder.Services.AddScoped<AsignarIncidenteCasoUso>();
builder.Services.AddScoped<CambiarEstadoIncidenteCasoUso>();
builder.Services.AddScoped<AgregarComentarioIncidenteCasoUso>();
builder.Services.AddScoped<ResolverIncidenteCasoUso>();
builder.Services.AddScoped<CerrarIncidenteCasoUso>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PdsTkDbContext>();
    await DbInicializador.InicializarAsync(dbContext);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
