using SocialCare.Application.DTOs.Programas;
using SocialCare.Domain.Entities;

namespace SocialCare.Application.Mappings;

public static class ProgramaMappings
{
    public static ProgramaResumoDto ToResumoDto(this ProgramaSocial p)
        => new(p.Id, p.Nome, p.OrgaoResponsavel, p.IconeBase64, p.ValorPadrao, p.Ativo);

    public static ProgramaResponse ToResponse(this ProgramaSocial p)
        => new(
            p.Id,
            p.Nome,
            p.OrgaoResponsavel,
            p.Descricao,
            p.Requisitos,
            p.IconeBase64,
            p.ValorPadrao,
            p.DuracaoMesesPadrao,
            p.VigenciaInicio,
            p.VigenciaFim,
            p.Ativo,
            p.CriadoEm,
            p.AtualizadoEm);
}
