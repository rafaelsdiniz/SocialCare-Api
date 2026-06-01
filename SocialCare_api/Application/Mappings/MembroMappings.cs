using SocialCare.Application.DTOs.Membros;
using SocialCare.Domain.Entities;

namespace SocialCare.Application.Mappings;

public static class MembroMappings
{
    public static int CalcularIdade(this DateTime nascimento, DateTime? referencia = null)
    {
        var hoje = (referencia ?? DateTime.Today).Date;
        var idade = hoje.Year - nascimento.Year;
        if (nascimento.Date > hoje.AddYears(-idade)) idade--;
        return idade < 0 ? 0 : idade;
    }

    public static decimal RendaConsiderada(this Membro m)
        => m.Rendas
            .Where(r => r.TipoRenda is not null && r.TipoRenda.ConsideradaParaCalculo)
            .Sum(r => r.Valor);

    public static DocumentoDto ToDto(this Documento d)
        => new(d.Id, d.TipoDocumentoId, d.TipoDocumento?.Sigla, d.Numero, d.OrgaoEmissor, d.DataEmissao, d.DataValidade);

    public static RendaDto ToDto(this Renda r)
        => new(
            r.Id,
            r.TipoRendaId,
            r.TipoRenda?.Nome,
            r.Valor,
            r.MesReferencia,
            r.AnoReferencia,
            r.Fonte,
            r.Observacao,
            r.TipoRenda?.ConsideradaParaCalculo ?? false);

    public static MembroResumoDto ToResumoDto(this Membro m)
        => new(
            m.Id,
            m.Nome,
            m.DataNascimento.CalcularIdade(),
            m.Sexo.ToString(),
            m.Parentesco?.Nome,
            m.RendaConsiderada());

    public static MembroResponse ToResponse(this Membro m)
        => new(
            m.Id,
            m.FamiliaId,
            m.Nome,
            m.DataNascimento,
            m.DataNascimento.CalcularIdade(),
            m.Sexo.ToString(),
            m.EstadoCivil.ToString(),
            m.ParentescoId,
            m.Parentesco?.Nome,
            m.NomeMae,
            m.NomePai,
            m.Escolaridade,
            m.Ocupacao,
            m.PessoaComDeficiencia,
            m.DescricaoDeficiencia,
            m.Telefone,
            m.RendaConsiderada(),
            m.Documentos.Select(d => d.ToDto()).ToList(),
            m.Rendas.Select(r => r.ToDto()).ToList(),
            m.CriadoEm,
            m.AtualizadoEm,
            m.Ativo);
}
