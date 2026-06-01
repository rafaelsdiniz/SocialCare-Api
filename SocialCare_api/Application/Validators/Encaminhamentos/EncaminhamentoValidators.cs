using FluentValidation;
using SocialCare.Application.DTOs.Encaminhamentos;

namespace SocialCare.Application.Validators.Encaminhamentos;

public class CriarEncaminhamentoRequestValidator : AbstractValidator<CriarEncaminhamentoRequest>
{
    public CriarEncaminhamentoRequestValidator()
    {
        RuleFor(x => x.FamiliaId).GreaterThan(0).WithMessage("Informe a família.");
        RuleFor(x => x.InstituicaoParceiraId).GreaterThan(0).WithMessage("Informe a instituição parceira.");
        RuleFor(x => x.Motivo).NotEmpty().WithMessage("O motivo do encaminhamento é obrigatório.").MaximumLength(255);
        RuleFor(x => x.Demanda).MaximumLength(1000);
    }
}

public class RegistrarRetornoRequestValidator : AbstractValidator<RegistrarRetornoRequest>
{
    public RegistrarRetornoRequestValidator()
    {
        RuleFor(x => x.Status).IsInEnum().WithMessage("Status inválido.");
        RuleFor(x => x.Retorno).MaximumLength(1000);
    }
}
