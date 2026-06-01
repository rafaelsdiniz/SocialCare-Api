using FluentValidation;
using SocialCare.Application.DTOs.Programas;

namespace SocialCare.Application.Validators.Programas;

public class CriarProgramaRequestValidator : AbstractValidator<CriarProgramaRequest>
{
    public CriarProgramaRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().WithMessage("O nome do programa é obrigatório.").MaximumLength(120);
        RuleFor(x => x.OrgaoResponsavel).NotEmpty().WithMessage("O órgão responsável é obrigatório.").MaximumLength(120);
        RuleFor(x => x.Descricao).MaximumLength(1000);
        RuleFor(x => x.Requisitos).MaximumLength(1000);
        RuleFor(x => x.ValorPadrao).GreaterThanOrEqualTo(0).When(x => x.ValorPadrao.HasValue)
            .WithMessage("O valor padrão não pode ser negativo.");
        RuleFor(x => x.DuracaoMesesPadrao).GreaterThan(0).When(x => x.DuracaoMesesPadrao.HasValue)
            .WithMessage("A duração padrão deve ser maior que zero.");
        RuleFor(x => x.VigenciaFim).GreaterThan(x => x.VigenciaInicio)
            .When(x => x.VigenciaInicio.HasValue && x.VigenciaFim.HasValue)
            .WithMessage("O fim da vigência deve ser posterior ao início.");
    }
}

public class AtualizarProgramaRequestValidator : AbstractValidator<AtualizarProgramaRequest>
{
    public AtualizarProgramaRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().WithMessage("O nome do programa é obrigatório.").MaximumLength(120);
        RuleFor(x => x.OrgaoResponsavel).NotEmpty().WithMessage("O órgão responsável é obrigatório.").MaximumLength(120);
        RuleFor(x => x.Descricao).MaximumLength(1000);
        RuleFor(x => x.Requisitos).MaximumLength(1000);
        RuleFor(x => x.ValorPadrao).GreaterThanOrEqualTo(0).When(x => x.ValorPadrao.HasValue)
            .WithMessage("O valor padrão não pode ser negativo.");
        RuleFor(x => x.DuracaoMesesPadrao).GreaterThan(0).When(x => x.DuracaoMesesPadrao.HasValue)
            .WithMessage("A duração padrão deve ser maior que zero.");
        RuleFor(x => x.VigenciaFim).GreaterThan(x => x.VigenciaInicio)
            .When(x => x.VigenciaInicio.HasValue && x.VigenciaFim.HasValue)
            .WithMessage("O fim da vigência deve ser posterior ao início.");
    }
}
