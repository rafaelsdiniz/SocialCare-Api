using Microsoft.EntityFrameworkCore;
using SocialCare.Domain.Entities;

namespace SocialCare.Infrastructure.Data;

internal static class DataSeeder
{
    private static readonly DateTime SeedDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static void Seed(ModelBuilder mb)
    {
        SeedPerfis(mb);
        SeedParentescos(mb);
        SeedTiposDocumento(mb);
        SeedTiposRenda(mb);
        SeedVulnerabilidades(mb);
        SeedProgramasSociais(mb);
    }

    private static void SeedPerfis(ModelBuilder mb)
    {
        mb.Entity<Perfil>().HasData(
            new Perfil { Id = 1, Nome = "Administrador", Descricao = "Acesso total ao sistema", CriadoEm = SeedDate, Ativo = true },
            new Perfil { Id = 2, Nome = "Gestor", Descricao = "Programas, benefícios e relatórios", CriadoEm = SeedDate, Ativo = true },
            new Perfil { Id = 3, Nome = "AssistenteSocial", Descricao = "Atuação direta com famílias e visitas", CriadoEm = SeedDate, Ativo = true }
        );
    }

    private static void SeedParentescos(ModelBuilder mb)
    {
        mb.Entity<Parentesco>().HasData(
            new Parentesco { Id = 1, Nome = "Responsável", Descricao = "Responsável familiar", CriadoEm = SeedDate, Ativo = true },
            new Parentesco { Id = 2, Nome = "Cônjuge", CriadoEm = SeedDate, Ativo = true },
            new Parentesco { Id = 3, Nome = "Filho(a)", CriadoEm = SeedDate, Ativo = true },
            new Parentesco { Id = 4, Nome = "Enteado(a)", CriadoEm = SeedDate, Ativo = true },
            new Parentesco { Id = 5, Nome = "Pai/Mãe", CriadoEm = SeedDate, Ativo = true },
            new Parentesco { Id = 6, Nome = "Avô/Avó", CriadoEm = SeedDate, Ativo = true },
            new Parentesco { Id = 7, Nome = "Neto(a)", CriadoEm = SeedDate, Ativo = true },
            new Parentesco { Id = 8, Nome = "Irmão/Irmã", CriadoEm = SeedDate, Ativo = true },
            new Parentesco { Id = 9, Nome = "Outro", CriadoEm = SeedDate, Ativo = true }
        );
    }

    private static void SeedTiposDocumento(ModelBuilder mb)
    {
        mb.Entity<TipoDocumento>().HasData(
            new TipoDocumento { Id = 1, Nome = "CPF", Sigla = "CPF", MascaraValidacao = "###.###.###-##", CriadoEm = SeedDate, Ativo = true },
            new TipoDocumento { Id = 2, Nome = "RG", Sigla = "RG", CriadoEm = SeedDate, Ativo = true },
            new TipoDocumento { Id = 3, Nome = "Número de Identificação Social", Sigla = "NIS", CriadoEm = SeedDate, Ativo = true },
            new TipoDocumento { Id = 4, Nome = "Certidão de Nascimento", Sigla = "CertidaoNasc", CriadoEm = SeedDate, Ativo = true },
            new TipoDocumento { Id = 5, Nome = "Carteira de Trabalho", Sigla = "CTPS", CriadoEm = SeedDate, Ativo = true },
            new TipoDocumento { Id = 6, Nome = "Título de Eleitor", Sigla = "TituloEleitor", CriadoEm = SeedDate, Ativo = true }
        );
    }

    private static void SeedTiposRenda(ModelBuilder mb)
    {
        mb.Entity<TipoRenda>().HasData(
            new TipoRenda { Id = 1, Nome = "Salário CLT", ConsideradaParaCalculo = true, CriadoEm = SeedDate, Ativo = true },
            new TipoRenda { Id = 2, Nome = "Autônomo", ConsideradaParaCalculo = true, CriadoEm = SeedDate, Ativo = true },
            new TipoRenda { Id = 3, Nome = "Aposentadoria", ConsideradaParaCalculo = true, CriadoEm = SeedDate, Ativo = true },
            new TipoRenda { Id = 4, Nome = "Pensão", ConsideradaParaCalculo = true, CriadoEm = SeedDate, Ativo = true },
            new TipoRenda { Id = 5, Nome = "Bolsa Família", ConsideradaParaCalculo = false, CriadoEm = SeedDate, Ativo = true },
            new TipoRenda { Id = 6, Nome = "Benefício de Prestação Continuada", ConsideradaParaCalculo = true, CriadoEm = SeedDate, Ativo = true },
            new TipoRenda { Id = 7, Nome = "Trabalho Informal", ConsideradaParaCalculo = true, CriadoEm = SeedDate, Ativo = true }
        );
    }

    private static void SeedVulnerabilidades(ModelBuilder mb)
    {
        mb.Entity<Vulnerabilidade>().HasData(
            new Vulnerabilidade { Id = 1, Nome = "Insegurança alimentar", Severidade = 3, CriadoEm = SeedDate, Ativo = true },
            new Vulnerabilidade { Id = 2, Nome = "Moradia precária", Severidade = 3, CriadoEm = SeedDate, Ativo = true },
            new Vulnerabilidade { Id = 3, Nome = "Violência doméstica", Severidade = 4, CriadoEm = SeedDate, Ativo = true },
            new Vulnerabilidade { Id = 4, Nome = "Trabalho infantil", Severidade = 4, CriadoEm = SeedDate, Ativo = true },
            new Vulnerabilidade { Id = 5, Nome = "Situação de rua", Severidade = 4, CriadoEm = SeedDate, Ativo = true },
            new Vulnerabilidade { Id = 6, Nome = "Desemprego prolongado", Severidade = 2, CriadoEm = SeedDate, Ativo = true },
            new Vulnerabilidade { Id = 7, Nome = "Evasão escolar", Severidade = 3, CriadoEm = SeedDate, Ativo = true },
            new Vulnerabilidade { Id = 8, Nome = "Dependência química", Severidade = 4, CriadoEm = SeedDate, Ativo = true },
            new Vulnerabilidade { Id = 9, Nome = "Idoso em situação de risco", Severidade = 4, CriadoEm = SeedDate, Ativo = true },
            new Vulnerabilidade { Id = 10, Nome = "Gestante sem acompanhamento", Severidade = 3, CriadoEm = SeedDate, Ativo = true }
        );
    }

    private static void SeedProgramasSociais(ModelBuilder mb)
    {
        mb.Entity<ProgramaSocial>().HasData(
            new ProgramaSocial { Id = 1, Nome = "Bolsa Família", OrgaoResponsavel = "MDS", Descricao = "Transferência de renda para famílias em vulnerabilidade", CriadoEm = SeedDate, Ativo = true },
            new ProgramaSocial { Id = 2, Nome = "Auxílio Gás", OrgaoResponsavel = "MDS", Descricao = "Auxílio para compra de gás de cozinha", CriadoEm = SeedDate, Ativo = true },
            new ProgramaSocial { Id = 3, Nome = "BPC - Benefício de Prestação Continuada", OrgaoResponsavel = "INSS", CriadoEm = SeedDate, Ativo = true },
            new ProgramaSocial { Id = 4, Nome = "Programa Leite das Crianças", OrgaoResponsavel = "Secretaria Estadual", CriadoEm = SeedDate, Ativo = true },
            new ProgramaSocial { Id = 5, Nome = "Tarifa Social de Energia", OrgaoResponsavel = "ANEEL", CriadoEm = SeedDate, Ativo = true }
        );
    }
}
