using FluentValidation;
using SocialCare.Application.DTOs.Visitas;

namespace SocialCare.Application.Validators.Visitas;

public class AgendarVisitaRequestValidator : AbstractValidator<AgendarVisitaRequest>
{
    public AgendarVisitaRequestValidator()
    {
        RuleFor(x => x.FamiliaId).GreaterThan(0).WithMessage("Informe a família.");
        RuleFor(x => x.DataAgendada).NotEmpty().WithMessage("A data agendada é obrigatória.");
        RuleFor(x => x.Tipo).IsInEnum().WithMessage("Tipo de visita inválido.");
        RuleFor(x => x.Motivo).MaximumLength(255);
    }
}

public class AtualizarVisitaRequestValidator : AbstractValidator<AtualizarVisitaRequest>
{
    public AtualizarVisitaRequestValidator()
    {
        RuleFor(x => x.DataAgendada).NotEmpty().WithMessage("A data agendada é obrigatória.");
        RuleFor(x => x.Tipo).IsInEnum().WithMessage("Tipo de visita inválido.");
        RuleFor(x => x.Motivo).MaximumLength(255);
        RuleFor(x => x.Observacoes).MaximumLength(1000);
    }
}

public class RegistrarVisitaRequestValidator : AbstractValidator<RegistrarVisitaRequest>
{
    public RegistrarVisitaRequestValidator()
    {
        RuleFor(x => x.Observacoes).MaximumLength(1000);
        RuleFor(x => x.Encaminhamentos).MaximumLength(1000);
    }
}
