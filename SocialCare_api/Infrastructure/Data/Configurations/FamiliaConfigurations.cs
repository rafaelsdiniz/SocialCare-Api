using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialCare.Domain.Entities;

namespace SocialCare.Infrastructure.Data.Configurations;

public class FamiliaConfiguration : IEntityTypeConfiguration<Familia>
{
    public void Configure(EntityTypeBuilder<Familia> b)
    {
        b.ToTable("familia");
        b.HasKey(x => x.Id);
        b.Property(x => x.CodigoFamiliar).HasMaxLength(20).IsRequired();
        b.Property(x => x.NomeResponsavel).HasMaxLength(150).IsRequired();
        b.Property(x => x.RendaTotalMensal).HasPrecision(12, 2);
        b.Property(x => x.RendaPerCapita).HasPrecision(12, 2);
        b.Property(x => x.Observacoes).HasMaxLength(1000);
        b.Property(x => x.Status).HasConversion<int>();
        b.HasIndex(x => x.CodigoFamiliar).IsUnique();

        b.HasOne(x => x.MembroResponsavel)
            .WithMany()
            .HasForeignKey(x => x.MembroResponsavelId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

public class ParentescoConfiguration : IEntityTypeConfiguration<Parentesco>
{
    public void Configure(EntityTypeBuilder<Parentesco> b)
    {
        b.ToTable("parentesco");
        b.HasKey(x => x.Id);
        b.Property(x => x.Nome).HasMaxLength(50).IsRequired();
        b.Property(x => x.Descricao).HasMaxLength(255);
        b.HasIndex(x => x.Nome).IsUnique();
    }
}

public class MembroConfiguration : IEntityTypeConfiguration<Membro>
{
    public void Configure(EntityTypeBuilder<Membro> b)
    {
        b.ToTable("membro");
        b.HasKey(x => x.Id);
        b.Property(x => x.Nome).HasMaxLength(150).IsRequired();
        b.Property(x => x.NomeMae).HasMaxLength(150);
        b.Property(x => x.NomePai).HasMaxLength(150);
        b.Property(x => x.Escolaridade).HasMaxLength(80);
        b.Property(x => x.Ocupacao).HasMaxLength(100);
        b.Property(x => x.DescricaoDeficiencia).HasMaxLength(255);
        b.Property(x => x.Telefone).HasMaxLength(20);
        b.Property(x => x.Sexo).HasConversion<int>();
        b.Property(x => x.EstadoCivil).HasConversion<int>();

        b.HasOne(x => x.Familia)
            .WithMany(f => f.Membros)
            .HasForeignKey(x => x.FamiliaId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Parentesco)
            .WithMany(p => p.Membros)
            .HasForeignKey(x => x.ParentescoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class TipoDocumentoConfiguration : IEntityTypeConfiguration<TipoDocumento>
{
    public void Configure(EntityTypeBuilder<TipoDocumento> b)
    {
        b.ToTable("tipo_documento");
        b.HasKey(x => x.Id);
        b.Property(x => x.Nome).HasMaxLength(80).IsRequired();
        b.Property(x => x.Sigla).HasMaxLength(20).IsRequired();
        b.Property(x => x.MascaraValidacao).HasMaxLength(100);
        b.HasIndex(x => x.Sigla).IsUnique();
    }
}

public class DocumentoConfiguration : IEntityTypeConfiguration<Documento>
{
    public void Configure(EntityTypeBuilder<Documento> b)
    {
        b.ToTable("documento");
        b.HasKey(x => x.Id);
        b.Property(x => x.Numero).HasMaxLength(50).IsRequired();
        b.Property(x => x.OrgaoEmissor).HasMaxLength(80);

        b.HasOne(x => x.Membro)
            .WithMany(m => m.Documentos)
            .HasForeignKey(x => x.MembroId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.TipoDocumento)
            .WithMany(td => td.Documentos)
            .HasForeignKey(x => x.TipoDocumentoId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => new { x.TipoDocumentoId, x.Numero });
    }
}
