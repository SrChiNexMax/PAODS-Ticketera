using Microsoft.EntityFrameworkCore;
using PdsTk.Dominio.Entidades;

namespace PdsTk.Infraestructura.Persistencia;

public class PdsTkDbContext : DbContext
{
    public PdsTkDbContext(DbContextOptions<PdsTkDbContext> options) : base(options)
    {
    }

    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<PoliticaSLA> PoliticasSLA => Set<PoliticaSLA>();
    public DbSet<Incidente> Incidentes => Set<Incidente>();
    public DbSet<ComentarioIncidente> ComentariosIncidente => Set<ComentarioIncidente>();
    public DbSet<RegistroAuditoria> RegistrosAuditoria => Set<RegistroAuditoria>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PdsTkDbContext).Assembly);
    }
}