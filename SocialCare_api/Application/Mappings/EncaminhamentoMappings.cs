using SocialCare.Application.DTOs.Encaminhamentos;
using SocialCare.Domain.Entities;

namespace SocialCare.Application.Mappings;

public static class EncaminhamentoMappings
{
    public static EncaminhamentoResumoDto ToResumoDto(this Encaminhamento e)
        => new(
            e.Id,
            e.FamiliaId,
            e.Familia?.CodigoFamiliar,
            e.InstituicaoParceiraId,
            e.InstituicaoParceira?.Nome,
            e.Motivo,
            e.Status.ToString(),
            e.DataEncaminhamento);

    public static EncaminhamentoResponse ToResponse(this Encaminhamento e)
        => new(
            e.Id,
            e.FamiliaId,
            e.Familia?.CodigoFamiliar,
            e.InstituicaoParceiraId,
            e.InstituicaoParceira?.Nome,
            e.MembroId,
            e.Membro?.Nome,
            e.Motivo,
            e.Demanda,
            e.Status.ToString(),
            e.DataEncaminhamento,
            e.DataRetorno,
            e.Retorno,
            e.CriadoEm,
            e.AtualizadoEm);
}
