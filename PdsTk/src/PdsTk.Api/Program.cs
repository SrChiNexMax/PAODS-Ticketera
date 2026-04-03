using Microsoft.EntityFrameworkCore;
using PdsTk.Infraestructura.Persistencia;

var builder = WebApplication.CreateBuilder(args);

// Servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PdsTkDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSqlServer")));

var app = builder.Build();

// Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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