using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialCare.Domain.Entities;

namespace SocialCare.Infrastructure.Data.Configurations;

public class VisitaConfiguration : IEntityTypeConfiguration<Visita>
{
    public void Configure(EntityTypeBuilder<Visita> b)
    {
        b.ToTable("visita");
        b.HasKey(x => x.Id);
        b.Property(x => x.Motivo).HasMaxLength(255);
        b.Property(x => x.Observacoes).HasMaxLength(1000);
        b.Property(x => x.Encaminhamentos).HasMaxLength(1000);
        b.Property(x => x.Tipo).HasConversion<int>();
        b.Property(x => x.Status).HasConversion<int>();

        b.HasOne(x => x.Familia)
            .WithMany(f => f.Visitas)
            .HasForeignKey(x => x.FamiliaId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.AssistenteResponsavel)
            .WithMany(u => u.VisitasResponsavel)
            .HasForeignKey(x => x.AssistenteResponsavelId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.DataAgendada);
    }
}

public class AtendimentoConfiguration : IEntityTypeConfiguration<Atendimento>
{
    public void Configure(EntityTypeBuilder<Atendimento> b)
    {
        b.ToTable("atendimento");
        b.HasKey(x => x.Id);
        b.Property(x => x.Motivo).HasMaxLength(255).IsRequired();
        b.Property(x => x.Parecer).HasMaxLength(1000);
        b.Property(x => x.Demanda).HasMaxLength(1000);
        b.Property(x => x.Status).HasConversion<int>();

        b.HasOne(x => x.Familia)
            .WithMany(f => f.Atendimentos)
            .HasForeignKey(x => x.FamiliaId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.AssistenteResponsavel)
            .WithMany(u => u.AtendimentosResponsavel)
            .HasForeignKey(x => x.AssistenteResponsavelId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.DataAtendimento);
    }
}

public class EncaminhamentoConfiguration : IEntityTypeConfiguration<Encaminhamento>
{
    public void Configure(EntityTypeBuilder<Encaminhamento> b)
    {
        b.ToTable("encaminhamento");
        b.HasKey(x => x.Id);
        b.Property(x => x.Motivo).HasMaxLength(255).IsRequired();
        b.Property(x => x.Demanda).HasMaxLength(1000);
        b.Property(x => x.Retorno).HasMaxLength(1000);
        b.Property(x => x.Status).HasConversion<int>();

        b.HasOne(x => x.Familia)
            .WithMany(f => f.Encaminhamentos)
            .HasForeignKey(x => x.FamiliaId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.InstituicaoParceira)
            .WithMany(i => i.Encaminhamentos)
            .HasForeignKey(x => x.InstituicaoParceiraId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Membro)
            .WithMany()
            .HasForeignKey(x => x.MembroId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasIndex(x => x.DataEncaminhamento);
    }
}
