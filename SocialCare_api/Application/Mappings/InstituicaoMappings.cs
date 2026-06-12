using SocialCare.Application.DTOs.Instituicoes;
using SocialCare.Domain.Entities;

namespace SocialCare.Application.Mappings;

public static class InstituicaoMappings
{
    public static InstituicaoResumoDto ToResumoDto(this InstituicaoParceira i)
        => new(i.Id, i.Nome, i.Cnpj, i.AreaAtuacao, i.Ativo);

    public static InstituicaoResponse ToResponse(this InstituicaoParceira i)
        => new(
            i.Id,
            i.Nome,
            i.Cnpj,
            i.AreaAtuacao,
            i.Telefone,
            i.Email,
            i.ResponsavelContato,
            i.EnderecoCompleto,
            i.Ativo,
            i.CriadoEm,
            i.AtualizadoEm);
}
