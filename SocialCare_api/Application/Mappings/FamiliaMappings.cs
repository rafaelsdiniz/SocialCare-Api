using SocialCare.Application.DTOs.Familias;
using SocialCare.Domain.Entities;

namespace SocialCare.Application.Mappings;

/// <summary>Mapeamento manual entre entidades de família e seus DTOs.</summary>
public static class FamiliaMappings
{
    public static EnderecoDto? ToDto(this Endereco? e)
        => e is null
            ? null
            : new EnderecoDto(
                e.Id,
                e.Cep,
                e.Logradouro,
                e.Numero,
                e.Complemento,
                e.Bairro,
                e.PontoReferencia,
                e.MunicipioId,
                e.Municipio?.Nome,
                e.Municipio?.Estado?.Sigla);

    public static FamiliaResponse ToResponse(this Familia f)
        => new(
            f.Id,
            f.CodigoFamiliar,
            f.NomeResponsavel,
            f.QuantidadeMembros,
            f.RendaTotalMensal,
            f.RendaPerCapita,
            f.Status.ToString(),
            f.Observacoes,
            f.DataCadastro,
            f.Endereco.ToDto(),
            f.CriadoEm,
            f.AtualizadoEm,
            f.Ativo);

    public static FamiliaResumoDto ToResumoDto(this Familia f)
        => new(
            f.Id,
            f.CodigoFamiliar,
            f.NomeResponsavel,
            f.QuantidadeMembros,
            f.RendaPerCapita,
            f.Status.ToString(),
            f.Endereco?.Municipio?.Nome,
            f.Endereco?.Municipio?.Estado?.Sigla);
}
