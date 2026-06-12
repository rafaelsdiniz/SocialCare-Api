using SocialCare.Application.DTOs.Beneficios;
using SocialCare.Domain.Entities;

namespace SocialCare.Application.Mappings;

public static class BeneficioMappings
{
    public static BeneficioResumoDto ToResumoDto(this Beneficio b)
        => new(
            b.Id,
            b.FamiliaId,
            b.Familia?.CodigoFamiliar,
            b.ProgramaSocialId,
            b.ProgramaSocial?.Nome,
            b.Valor,
            b.Status.ToString(),
            b.DataInicio,
            b.DataFim);

    public static BeneficioResponse ToResponse(this Beneficio b)
        => new(
            b.Id,
            b.FamiliaId,
            b.Familia?.CodigoFamiliar,
            b.Familia?.NomeResponsavel,
            b.ProgramaSocialId,
            b.ProgramaSocial?.Nome,
            b.Valor,
            b.Status.ToString(),
            b.DataInicio,
            b.DataFim,
            b.Observacao,
            b.MotivoEncerramento,
            b.AprovadoPorUsuarioId,
            b.AprovadoPor?.Nome,
            b.CriadoEm,
            b.AtualizadoEm);
}
