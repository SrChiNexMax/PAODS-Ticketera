using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PdsTk.Dominio.Entidades;

namespace PdsTk.Infraestructura.Persistencia.Configuraciones;

public class IncidenteConfiguracion : IEntityTypeConfiguration<Incidente>
{
    public void Configure(EntityTypeBuilder<Incidente> builder)
    {
        builder.ToTable("Incidentes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Codigo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Titulo)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Descripcion)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.Estado)
            .IsRequired();

        builder.Property(x => x.Prioridad)
            .IsRequired();

        builder.Property(x => x.FechaCreacion)
            .IsRequired();

        builder.Property(x => x.FechaLimitePrimeraRespuesta)
            .IsRequired();

        builder.Property(x => x.FechaLimiteResolucion)
            .IsRequired();

        builder.Property(x => x.Activo)
            .IsRequired();

        builder.HasIndex(x => x.Codigo)
            .IsUnique();

        builder.HasIndex(x => x.Estado);

        builder.HasIndex(x => x.Prioridad);

        builder.HasOne(x => x.Solicitante)
            .WithMany(x => x.IncidentesSolicitados)
            .HasForeignKey(x => x.SolicitanteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AgenteAsignado)
            .WithMany(x => x.IncidentesAsignados)
            .HasForeignKey(x => x.AgenteAsignadoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PoliticaSLA)
            .WithMany(x => x.Incidentes)
            .HasForeignKey(x => x.PoliticaSLAId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Comentarios)
            .WithOne(x => x.Incidente)
            .HasForeignKey(x => x.IncidenteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.RegistrosAuditoria)
            .WithOne(x => x.Incidente)
            .HasForeignKey(x => x.IncidenteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}