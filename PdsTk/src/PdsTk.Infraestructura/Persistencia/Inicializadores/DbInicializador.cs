using Microsoft.EntityFrameworkCore;
using PdsTk.Dominio.Entidades;
using PdsTk.Dominio.Enumeradores;
using PdsTk.Infraestructura.Seguridad;

namespace PdsTk.Infraestructura.Persistencia.Inicializadores;

public static class DbInicializador
{
    public static async Task InicializarAsync(PdsTkDbContext context)
    {
        await context.Database.MigrateAsync();

        await SeedRolesAsync(context);
        await SeedUsuariosAsync(context);
        await SeedPoliticasSlaAsync(context);
    }

    private static async Task SeedRolesAsync(PdsTkDbContext context)
    {
        if (await context.Roles.AnyAsync())
            return;

        var roles = new List<Rol>
        {
            new Rol
            {
                Nombre = "Administrador",
                Descripcion = "Gestiona catálogos, usuarios, SLA y configuración general",
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Rol
            {
                Nombre = "Supervisor",
                Descripcion = "Gestiona y atiende incidentes asignados",
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Rol
            {
                Nombre = "Solicitante",
                Descripcion = "Registra incidentes y da seguimiento a sus solicitudes",
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            }
        };

        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();
    }

    private static async Task SeedUsuariosAsync(PdsTkDbContext context)
    {
        if (await context.Usuarios.AnyAsync())
            return;

        var rolAdministrador = await context.Roles.FirstAsync(r => r.Nombre == "Administrador");
        var rolSupervisor = await context.Roles.FirstAsync(r => r.Nombre == "Supervisor");
        var rolSolicitante = await context.Roles.FirstAsync(r => r.Nombre == "Solicitante");

        var usuarios = new List<Usuario>
        {
            new Usuario
            {
                Nombre = "Admin Principal",
                Correo = "admin@pdstk.com",
                Clave = PasswordHelper.GenerarHash("Admin123*"),
                RolId = rolAdministrador.Id,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Usuario
            {
                Nombre = "Sofia Supervisor",
                Correo = "supervisor@pdstk.com",
                Clave = PasswordHelper.GenerarHash("Supervisor123*"),
                RolId = rolSupervisor.Id,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Usuario
            {
                Nombre = "Carlos Solicitante",
                Correo = "solicitante@pdstk.com",
                Clave = PasswordHelper.GenerarHash("Solicitante123*"),
                RolId = rolSolicitante.Id,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            }
        };

        await context.Usuarios.AddRangeAsync(usuarios);
        await context.SaveChangesAsync();
    }

    private static async Task SeedPoliticasSlaAsync(PdsTkDbContext context)
    {
        if (await context.PoliticasSLA.AnyAsync())
            return;

        var politicas = new List<PoliticaSLA>
        {
            new PoliticaSLA
            {
                Nombre = "SLA Baja",
                Descripcion = "Incidentes de baja prioridad con tiempos de respuesta y resolución más largos",
                Prioridad = PrioridadIncidente.Baja,
                TiempoPrimeraRespuestaMinutos = TimeSpan.FromMinutes(240),
                TiempoResolucionMinutos = TimeSpan.FromMinutes(1440),
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new PoliticaSLA
            {
                Nombre = "SLA Media",
                Descripcion = "Incidentes de prioridad media con tiempos de respuesta y resolución moderados",
                Prioridad = PrioridadIncidente.Media,
                TiempoPrimeraRespuestaMinutos = TimeSpan.FromMinutes(120),
                TiempoResolucionMinutos = TimeSpan.FromMinutes(480),
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new PoliticaSLA
            {
                Nombre = "SLA Alta",
                Descripcion = "Incidentes de alta prioridad con tiempos de respuesta y resolución rápidos",
                Prioridad = PrioridadIncidente.Alta,
                TiempoPrimeraRespuestaMinutos = TimeSpan.FromMinutes(60),
                TiempoResolucionMinutos = TimeSpan.FromMinutes(240),
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new PoliticaSLA
            {
                Nombre = "SLA Crítica",
                Descripcion = "Incidentes críticos con tiempos de respuesta y resolución muy rápidos",
                Prioridad = PrioridadIncidente.Critica,
                TiempoPrimeraRespuestaMinutos = TimeSpan.FromMinutes(15),
                TiempoResolucionMinutos = TimeSpan.FromMinutes(60),
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            }
        };

        await context.PoliticasSLA.AddRangeAsync(politicas);
        await context.SaveChangesAsync();
    }
}