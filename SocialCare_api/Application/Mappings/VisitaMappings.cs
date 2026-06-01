using SocialCare.Application.DTOs.Visitas;
using SocialCare.Domain.Entities;

namespace SocialCare.Application.Mappings;

public static class VisitaMappings
{
    public static VisitaResumoDto ToResumoDto(this Visita v)
        => new(
            v.Id,
            v.FamiliaId,
            v.Familia?.CodigoFamiliar,
            v.Tipo.ToString(),
            v.Status.ToString(),
            v.DataAgendada,
            v.DataRealizacao,
            v.AssistenteResponsavel?.Nome);

    public static VisitaResponse ToResponse(this Visita v, string? aviso = null)
        => new(
            v.Id,
            v.FamiliaId,
            v.Familia?.CodigoFamiliar,
            v.AssistenteResponsavelId,
            v.AssistenteResponsavel?.Nome,
            v.Tipo.ToString(),
            v.Status.ToString(),
            v.DataAgendada,
            v.DataRealizacao,
            v.Motivo,
            v.Observacoes,
            v.Encaminhamentos,
            aviso,
            v.CriadoEm,
            v.AtualizadoEm);
}
