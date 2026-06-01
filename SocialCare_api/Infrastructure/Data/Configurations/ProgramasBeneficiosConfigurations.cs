using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialCare.Domain.Entities;

namespace SocialCare.Infrastructure.Data.Configurations;

public class ProgramaSocialConfiguration : IEntityTypeConfiguration<ProgramaSocial>
{
    public void Configure(EntityTypeBuilder<ProgramaSocial> b)
    {
        b.ToTable("programa_social");
        b.HasKey(x => x.Id);
        b.Property(x => x.Nome).HasMaxLength(120).IsRequired();
        b.Property(x => x.Descricao).HasMaxLength(1000);
        b.Property(x => x.Requisitos).HasMaxLength(1000);
        b.Property(x => x.OrgaoResponsavel).HasMaxLength(120).IsRequired();
        b.Property(x => x.ValorPadrao).HasPrecision(12, 2);
        b.HasIndex(x => x.Nome).IsUnique();
    }
}

public class BeneficioConfiguration : IEntityTypeConfiguration<Beneficio>
{
    public void Configure(EntityTypeBuilder<Beneficio> b)
    {
        b.ToTable("beneficio");
        b.HasKey(x => x.Id);
        b.Property(x => x.Valor).HasPrecision(12, 2).IsRequired();
        b.Property(x => x.Observacao).HasMaxLength(500);
        b.Property(x => x.MotivoEncerramento).HasMaxLength(255);
        b.Property(x => x.Status).HasConversion<int>();

        b.HasOne(x => x.Familia)
            .WithMany(f => f.Beneficios)
            .HasForeignKey(x => x.FamiliaId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.ProgramaSocial)
            .WithMany(p => p.Beneficios)
            .HasForeignKey(x => x.ProgramaSocialId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.AprovadoPor)
            .WithMany()
            .HasForeignKey(x => x.AprovadoPorUsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasIndex(x => new { x.FamiliaId, x.ProgramaSocialId, x.DataInicio });
    }
}

public class InstituicaoParceiraConfiguration : IEntityTypeConfiguration<InstituicaoParceira>
{
    public void Configure(EntityTypeBuilder<InstituicaoParceira> b)
    {
        b.ToTable("instituicao_parceira");
        b.HasKey(x => x.Id);
        b.Property(x => x.Nome).HasMaxLength(150).IsRequired();
        b.Property(x => x.Cnpj).HasMaxLength(18).IsRequired();
        b.Property(x => x.AreaAtuacao).HasMaxLength(120).IsRequired();
        b.Property(x => x.Telefone).HasMaxLength(20);
        b.Property(x => x.Email).HasMaxLength(200);
        b.Property(x => x.ResponsavelContato).HasMaxLength(150);
        b.Property(x => x.EnderecoCompleto).HasMaxLength(255);
        b.HasIndex(x => x.Cnpj).IsUnique();
    }
}
