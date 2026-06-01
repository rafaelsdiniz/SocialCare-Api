using FluentValidation;
using SocialCare.Application.Common;
using SocialCare.Application.DTOs.Instituicoes;

namespace SocialCare.Application.Validators.Instituicoes;

public class CriarInstituicaoRequestValidator : AbstractValidator<CriarInstituicaoRequest>
{
    public CriarInstituicaoRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().WithMessage("O nome da instituição é obrigatório.").MaximumLength(150);
        RuleFor(x => x.Cnpj).NotEmpty().WithMessage("O CNPJ é obrigatório.")
            .Must(DocumentoFiscal.CnpjValido).WithMessage("CNPJ inválido (dígitos verificadores não conferem).");
        RuleFor(x => x.AreaAtuacao).NotEmpty().WithMessage("A área de atuação é obrigatória.").MaximumLength(120);
        RuleFor(x => x.Telefone).MaximumLength(20);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("E-mail inválido.").MaximumLength(200);
        RuleFor(x => x.ResponsavelContato).MaximumLength(150);
        RuleFor(x => x.EnderecoCompleto).MaximumLength(255);
    }
}

public class AtualizarInstituicaoRequestValidator : AbstractValidator<AtualizarInstituicaoRequest>
{
    public AtualizarInstituicaoRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().WithMessage("O nome da instituição é obrigatório.").MaximumLength(150);
        RuleFor(x => x.Cnpj).NotEmpty().WithMessage("O CNPJ é obrigatório.")
            .Must(DocumentoFiscal.CnpjValido).WithMessage("CNPJ inválido (dígitos verificadores não conferem).");
        RuleFor(x => x.AreaAtuacao).NotEmpty().WithMessage("A área de atuação é obrigatória.").MaximumLength(120);
        RuleFor(x => x.Telefone).MaximumLength(20);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("E-mail inválido.").MaximumLength(200);
        RuleFor(x => x.ResponsavelContato).MaximumLength(150);
        RuleFor(x => x.EnderecoCompleto).MaximumLength(255);
    }
}
