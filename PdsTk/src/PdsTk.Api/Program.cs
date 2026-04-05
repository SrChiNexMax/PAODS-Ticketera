using Microsoft.EntityFrameworkCore;
using PdsTk.Infraestructura.Persistencia;
using PdsTk.Infraestructura.Persistencia.Inicializadores;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PdsTkDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSqlServer")));

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

//app.UseAuthorization();
app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/api/salud", () =>
{
    return Results.Ok(new
    {
        mensaje = "API PdsTk funcionando correctamente",
        fechaUtc = DateTime.UtcNow
    });
});

app.Run();