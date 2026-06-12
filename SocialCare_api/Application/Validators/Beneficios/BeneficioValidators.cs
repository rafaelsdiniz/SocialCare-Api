using FluentValidation;
using SocialCare.Application.DTOs.Beneficios;

namespace SocialCare.Application.Validators.Beneficios;

public class ConcederBeneficioRequestValidator : AbstractValidator<ConcederBeneficioRequest>
{
    public ConcederBeneficioRequestValidator()
    {
        RuleFor(x => x.FamiliaId).GreaterThan(0).WithMessage("Informe a família.");
        RuleFor(x => x.ProgramaSocialId).GreaterThan(0).WithMessage("Informe o programa social.");
        RuleFor(x => x.Valor).GreaterThanOrEqualTo(0).WithMessage("O valor não pode ser negativo.");
        RuleFor(x => x.DataFim).GreaterThan(x => x.DataInicio)
            .When(x => x.DataFim.HasValue)
            .WithMessage("A data de término deve ser posterior à de início.");
        RuleFor(x => x.Observacao).MaximumLength(500);
    }
}

public class AtualizarBeneficioRequestValidator : AbstractValidator<AtualizarBeneficioRequest>
{
    public AtualizarBeneficioRequestValidator()
    {
        RuleFor(x => x.Valor).GreaterThanOrEqualTo(0).WithMessage("O valor não pode ser negativo.");
        RuleFor(x => x.DataFim).GreaterThan(x => x.DataInicio)
            .When(x => x.DataFim.HasValue)
            .WithMessage("A data de término deve ser posterior à de início.");
        RuleFor(x => x.Observacao).MaximumLength(500);
    }
}
