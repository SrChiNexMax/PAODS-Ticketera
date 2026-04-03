using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PdsTk.Dominio.Entidades;

namespace PdsTk.Infraestructura.Persistencia.Configuraciones;

public class ComentarioIncidenteConfiguracion : IEntityTypeConfiguration<ComentarioIncidente>
{
    public void Configure(EntityTypeBuilder<ComentarioIncidente> builder)
    {
        builder.ToTable("ComentariosIncidente");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Contenido)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.EsInterno)
            .IsRequired();

        builder.Property(x => x.FechaCreacion)
            .IsRequired();

        builder.HasOne(x => x.Incidente)
            .WithMany(x => x.Comentarios)
            .HasForeignKey(x => x.IncidenteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Usuario)
            .WithMany(x => x.Comentarios)
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}