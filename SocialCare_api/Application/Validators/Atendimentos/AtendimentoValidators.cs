using FluentValidation;
using SocialCare.Application.DTOs.Atendimentos;
using SocialCare.Domain.Enums;

namespace SocialCare.Application.Validators.Atendimentos;

public class AbrirAtendimentoRequestValidator : AbstractValidator<AbrirAtendimentoRequest>
{
    public AbrirAtendimentoRequestValidator()
    {
        RuleFor(x => x.FamiliaId).GreaterThan(0).WithMessage("Informe a família.");
        RuleFor(x => x.Motivo).NotEmpty().WithMessage("O motivo do atendimento é obrigatório.").MaximumLength(255);
        RuleFor(x => x.Demanda).MaximumLength(1000);
    }
}

public class AtualizarAtendimentoRequestValidator : AbstractValidator<AtualizarAtendimentoRequest>
{
    public AtualizarAtendimentoRequestValidator()
    {
        RuleFor(x => x.Motivo).NotEmpty().WithMessage("O motivo do atendimento é obrigatório.").MaximumLength(255);
        RuleFor(x => x.Demanda).MaximumLength(1000);
        RuleFor(x => x.Parecer).MaximumLength(1000);
        RuleFor(x => x.Status).IsInEnum().WithMessage("Status inválido.");
        RuleFor(x => x.Parecer)
            .NotEmpty().When(x => x.Status == StatusAtendimento.Concluido)
            .WithMessage("Informe o parecer para concluir o atendimento.");
    }
}
