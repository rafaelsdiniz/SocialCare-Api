using SocialCare.Application.DTOs.Atendimentos;
using SocialCare.Domain.Entities;

namespace SocialCare.Application.Mappings;

public static class AtendimentoMappings
{
    public static AtendimentoResumoDto ToResumoDto(this Atendimento a)
        => new(
            a.Id,
            a.FamiliaId,
            a.Familia?.CodigoFamiliar,
            a.Motivo,
            a.Status.ToString(),
            a.Remoto,
            a.DataAtendimento,
            a.AssistenteResponsavel?.Nome);

    public static AtendimentoResponse ToResponse(this Atendimento a)
        => new(
            a.Id,
            a.FamiliaId,
            a.Familia?.CodigoFamiliar,
            a.AssistenteResponsavelId,
            a.AssistenteResponsavel?.Nome,
            a.Motivo,
            a.Demanda,
            a.Parecer,
            a.Remoto,
            a.Status.ToString(),
            a.DataAtendimento,
            a.CriadoEm,
            a.AtualizadoEm);
}
