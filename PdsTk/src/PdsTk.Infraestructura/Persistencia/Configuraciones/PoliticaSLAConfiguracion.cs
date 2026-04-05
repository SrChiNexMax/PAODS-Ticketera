using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PdsTk.Dominio.Entidades;

namespace PdsTk.Infraestructura.Persistencia.Configuraciones;

public class PoliticaSLAConfiguracion : IEntityTypeConfiguration<PoliticaSLA>
{
    public void Configure(EntityTypeBuilder<PoliticaSLA> builder)
    {
        builder.ToTable("PoliticaSLA");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nombre)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Descripcion)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Prioridad)
            .IsRequired();

        builder.Property(x => x.TiempoPrimeraRespuestaMinutos)
            .HasColumnType("int")
            .IsRequired();

        builder.Property(x => x.TiempoResolucionMinutos)
            .HasColumnType("int")
            .IsRequired();
            
        builder.Property(x => x.Activo)
            .IsRequired();

        builder.Property(x => x.FechaCreacion)
            .IsRequired();

        builder.HasMany(x => x.Incidentes)
            .WithOne(x => x.PoliticaSLA)
            .HasForeignKey(x => x.PoliticaSLAId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}