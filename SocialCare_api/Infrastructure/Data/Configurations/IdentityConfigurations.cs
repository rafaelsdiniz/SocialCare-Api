using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialCare.Domain.Entities;

namespace SocialCare.Infrastructure.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> b)
    {
        b.ToTable("usuario");
        b.HasKey(x => x.Id);
        b.Property(x => x.Nome).HasMaxLength(150).IsRequired();
        b.Property(x => x.Email).HasMaxLength(200).IsRequired();
        b.Property(x => x.Login).HasMaxLength(50).IsRequired();
        b.Property(x => x.SenhaHash).HasMaxLength(255).IsRequired();
        b.HasIndex(x => x.Email).IsUnique();
        b.HasIndex(x => x.Login).IsUnique();
    }
}

public class PerfilConfiguration : IEntityTypeConfiguration<Perfil>
{
    public void Configure(EntityTypeBuilder<Perfil> b)
    {
        b.ToTable("perfil");
        b.HasKey(x => x.Id);
        b.Property(x => x.Nome).HasMaxLength(50).IsRequired();
        b.Property(x => x.Descricao).HasMaxLength(255);
        b.HasIndex(x => x.Nome).IsUnique();
    }
}

public class UsuarioPerfilConfiguration : IEntityTypeConfiguration<UsuarioPerfil>
{
    public void Configure(EntityTypeBuilder<UsuarioPerfil> b)
    {
        b.ToTable("usuario_perfil");
        b.HasKey(x => new { x.UsuarioId, x.PerfilId });
        b.HasOne(x => x.Usuario)
            .WithMany(u => u.UsuarioPerfis)
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Perfil)
            .WithMany(p => p.UsuarioPerfis)
            .HasForeignKey(x => x.PerfilId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class LogAuditoriaConfiguration : IEntityTypeConfiguration<LogAuditoria>
{
    public void Configure(EntityTypeBuilder<LogAuditoria> b)
    {
        b.ToTable("log_auditoria");
        b.HasKey(x => x.Id);
        b.Property(x => x.Entidade).HasMaxLength(100).IsRequired();
        b.Property(x => x.EntidadeId).HasMaxLength(50);
        b.Property(x => x.EnderecoIp).HasMaxLength(45);
        b.Property(x => x.UserAgent).HasMaxLength(255);
        b.Property(x => x.DadosAntes).HasColumnType("nvarchar(max)");
        b.Property(x => x.DadosDepois).HasColumnType("nvarchar(max)");
        b.HasOne(x => x.Usuario)
            .WithMany(u => u.Auditorias)
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);
        b.HasIndex(x => new { x.Entidade, x.EntidadeId });
        b.HasIndex(x => x.CriadoEm);
    }
}
