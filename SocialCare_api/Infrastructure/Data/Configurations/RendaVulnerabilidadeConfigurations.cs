using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialCare.Domain.Entities;

namespace SocialCare.Infrastructure.Data.Configurations;

public class TipoRendaConfiguration : IEntityTypeConfiguration<TipoRenda>
{
    public void Configure(EntityTypeBuilder<TipoRenda> b)
    {
        b.ToTable("tipo_renda");
        b.HasKey(x => x.Id);
        b.Property(x => x.Nome).HasMaxLength(80).IsRequired();
        b.Property(x => x.Descricao).HasMaxLength(255);
        b.HasIndex(x => x.Nome).IsUnique();
    }
}

public class RendaConfiguration : IEntityTypeConfiguration<Renda>
{
    public void Configure(EntityTypeBuilder<Renda> b)
    {
        b.ToTable("renda");
        b.HasKey(x => x.Id);
        b.Property(x => x.Valor).HasPrecision(12, 2).IsRequired();
        b.Property(x => x.Fonte).HasMaxLength(120);
        b.Property(x => x.Observacao).HasMaxLength(255);

        b.HasOne(x => x.Membro)
            .WithMany(m => m.Rendas)
            .HasForeignKey(x => x.MembroId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.TipoRenda)
            .WithMany(t => t.Rendas)
            .HasForeignKey(x => x.TipoRendaId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => new { x.MembroId, x.AnoReferencia, x.MesReferencia });
    }
}

public class VulnerabilidadeConfiguration : IEntityTypeConfiguration<Vulnerabilidade>
{
    public void Configure(EntityTypeBuilder<Vulnerabilidade> b)
    {
        b.ToTable("vulnerabilidade");
        b.HasKey(x => x.Id);
        b.Property(x => x.Nome).HasMaxLength(120).IsRequired();
        b.Property(x => x.Descricao).HasMaxLength(500);
        b.HasIndex(x => x.Nome).IsUnique();
    }
}

public class FamiliaVulnerabilidadeConfiguration : IEntityTypeConfiguration<FamiliaVulnerabilidade>
{
    public void Configure(EntityTypeBuilder<FamiliaVulnerabilidade> b)
    {
        b.ToTable("familia_vulnerabilidade");
        b.HasKey(x => new { x.FamiliaId, x.VulnerabilidadeId });
        b.Property(x => x.Observacao).HasMaxLength(500);

        b.HasOne(x => x.Familia)
            .WithMany(f => f.Vulnerabilidades)
            .HasForeignKey(x => x.FamiliaId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Vulnerabilidade)
            .WithMany(v => v.Familias)
            .HasForeignKey(x => x.VulnerabilidadeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
