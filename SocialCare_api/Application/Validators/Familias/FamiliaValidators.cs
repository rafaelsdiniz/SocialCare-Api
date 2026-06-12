using FluentValidation;
using SocialCare.Application.DTOs.Familias;

namespace SocialCare.Application.Validators.Familias;

public class EnderecoRequestValidator : AbstractValidator<EnderecoRequest>
{
    public EnderecoRequestValidator()
    {
        RuleFor(x => x.Cep)
            .NotEmpty().WithMessage("O CEP é obrigatório.")
            .Matches(@"^\d{5}-?\d{3}$").WithMessage("O CEP deve ter 8 dígitos (ex.: 01001-000).");

        RuleFor(x => x.Logradouro).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Numero).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Complemento).MaximumLength(100);
        RuleFor(x => x.Bairro).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PontoReferencia).MaximumLength(200);
        RuleFor(x => x.MunicipioId).GreaterThan(0).WithMessage("Informe um município válido.");
    }
}

public class CriarFamiliaRequestValidator : AbstractValidator<CriarFamiliaRequest>
{
    public CriarFamiliaRequestValidator()
    {
        RuleFor(x => x.CodigoFamiliar)
            .NotEmpty().WithMessage("O código familiar é obrigatório.")
            .MaximumLength(20);

        RuleFor(x => x.NomeResponsavel)
            .NotEmpty().WithMessage("O nome do responsável é obrigatório.")
            .MaximumLength(150);

        RuleFor(x => x.Observacoes).MaximumLength(1000);
        RuleFor(x => x.Endereco).NotNull().WithMessage("O endereço é obrigatório.");
        RuleFor(x => x.Endereco).SetValidator(new EnderecoRequestValidator()).When(x => x.Endereco is not null);
    }
}

public class AtualizarFamiliaRequestValidator : AbstractValidator<AtualizarFamiliaRequest>
{
    public AtualizarFamiliaRequestValidator()
    {
        RuleFor(x => x.NomeResponsavel)
            .NotEmpty().WithMessage("O nome do responsável é obrigatório.")
            .MaximumLength(150);

        RuleFor(x => x.Status).IsInEnum().WithMessage("Status inválido.");
        RuleFor(x => x.Observacoes).MaximumLength(1000);
        RuleFor(x => x.Endereco).NotNull().WithMessage("O endereço é obrigatório.");
        RuleFor(x => x.Endereco).SetValidator(new EnderecoRequestValidator()).When(x => x.Endereco is not null);
    }
}
