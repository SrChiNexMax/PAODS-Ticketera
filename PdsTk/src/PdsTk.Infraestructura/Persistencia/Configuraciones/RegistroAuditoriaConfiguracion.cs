using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PdsTk.Dominio.Entidades;

namespace PdsTk.Infraestructura.Persistencia.Configuraciones;

public class RegistroAuditoriaConfiguracion : IEntityTypeConfiguration<RegistroAuditoria>
{
    public void Configure(EntityTypeBuilder<RegistroAuditoria> builder)
    {
        builder.ToTable("RegistrosAuditoria");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TipoAccion)
            .IsRequired();

        builder.Property(x => x.Descripcion)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.ValorAnterior)
            .HasMaxLength(1000);

        builder.Property(x => x.ValorNuevo)
            .HasMaxLength(1000);

        builder.Property(x => x.Fecha)
            .IsRequired();

        builder.HasOne(x => x.Incidente)
            .WithMany(x => x.RegistrosAuditoria)
            .HasForeignKey(x => x.IncidenteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Usuario)
            .WithMany(x => x.RegistrosAuditoria)
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}