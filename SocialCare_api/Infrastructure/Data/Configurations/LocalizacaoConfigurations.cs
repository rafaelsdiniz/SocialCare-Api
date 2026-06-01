using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialCare.Domain.Entities;

namespace SocialCare.Infrastructure.Data.Configurations;

public class EstadoConfiguration : IEntityTypeConfiguration<Estado>
{
    public void Configure(EntityTypeBuilder<Estado> b)
    {
        b.ToTable("estado");
        b.HasKey(x => x.Id);
        b.Property(x => x.Sigla).HasMaxLength(2).IsRequired();
        b.Property(x => x.Nome).HasMaxLength(80).IsRequired();
        b.Property(x => x.Regiao).HasMaxLength(20).IsRequired();
        b.HasIndex(x => x.Sigla).IsUnique();
        b.HasIndex(x => x.CodigoIbge).IsUnique();
    }
}

public class MunicipioConfiguration : IEntityTypeConfiguration<Municipio>
{
    public void Configure(EntityTypeBuilder<Municipio> b)
    {
        b.ToTable("municipio");
        b.HasKey(x => x.Id);
        b.Property(x => x.Nome).HasMaxLength(120).IsRequired();
        b.HasIndex(x => x.CodigoIbge).IsUnique();
        b.HasIndex(x => new { x.EstadoId, x.Nome });
        b.HasOne(x => x.Estado)
            .WithMany(e => e.Municipios)
            .HasForeignKey(x => x.EstadoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class EnderecoConfiguration : IEntityTypeConfiguration<Endereco>
{
    public void Configure(EntityTypeBuilder<Endereco> b)
    {
        b.ToTable("endereco");
        b.HasKey(x => x.Id);
        b.Property(x => x.Cep).HasMaxLength(9).IsRequired();
        b.Property(x => x.Logradouro).HasMaxLength(200).IsRequired();
        b.Property(x => x.Numero).HasMaxLength(20).IsRequired();
        b.Property(x => x.Complemento).HasMaxLength(100);
        b.Property(x => x.Bairro).HasMaxLength(100).IsRequired();
        b.Property(x => x.PontoReferencia).HasMaxLength(200);
        b.HasOne(x => x.Municipio)
            .WithMany(m => m.Enderecos)
            .HasForeignKey(x => x.MunicipioId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Familia)
            .WithOne(f => f.Endereco!)
            .HasForeignKey<Endereco>(x => x.FamiliaId)
            .OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => x.FamiliaId).IsUnique();
        b.HasIndex(x => x.Cep);
    }
}
