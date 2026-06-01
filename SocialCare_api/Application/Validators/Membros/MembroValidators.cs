using FluentValidation;
using SocialCare.Application.Common;
using SocialCare.Application.DTOs.Membros;

namespace SocialCare.Application.Validators.Membros;

public class DocumentoRequestValidator : AbstractValidator<DocumentoRequest>
{
    public DocumentoRequestValidator()
    {
        RuleFor(x => x.TipoDocumentoId).GreaterThan(0).WithMessage("Informe o tipo de documento.");
        RuleFor(x => x.Numero).NotEmpty().WithMessage("O número do documento é obrigatório.").MaximumLength(50);
        RuleFor(x => x.OrgaoEmissor).MaximumLength(80);

        RuleFor(x => x.Numero)
            .Must(DocumentoFiscal.CpfValido)
            .When(x => x.TipoDocumentoId == Catalogos.TipoDocumento.Cpf)
            .WithMessage("CPF inválido (dígitos verificadores não conferem).");

        RuleFor(x => x.DataValidade)
            .GreaterThan(x => x.DataEmissao)
            .When(x => x.DataEmissao.HasValue && x.DataValidade.HasValue)
            .WithMessage("A validade deve ser posterior à emissão.");
    }
}

public class RendaRequestValidator : AbstractValidator<RendaRequest>
{
    public RendaRequestValidator()
    {
        RuleFor(x => x.TipoRendaId).GreaterThan(0).WithMessage("Informe o tipo de renda.");
        RuleFor(x => x.Valor).GreaterThanOrEqualTo(0).WithMessage("O valor não pode ser negativo.");
        RuleFor(x => x.MesReferencia).InclusiveBetween(1, 12).WithMessage("Mês de referência inválido (1 a 12).");
        RuleFor(x => x.AnoReferencia).InclusiveBetween(2000, DateTime.Today.Year)
            .WithMessage($"Ano de referência inválido (2000 a {DateTime.Today.Year}).");
        RuleFor(x => x.Fonte).MaximumLength(120);
        RuleFor(x => x.Observacao).MaximumLength(255);
    }
}

public class CriarMembroRequestValidator : AbstractValidator<CriarMembroRequest>
{
    public CriarMembroRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().WithMessage("O nome do membro é obrigatório.").MaximumLength(150);
        RuleFor(x => x.DataNascimento)
            .NotEmpty().WithMessage("A data de nascimento é obrigatória.")
            .LessThanOrEqualTo(DateTime.Today).WithMessage("A data de nascimento não pode ser futura.");
        RuleFor(x => x.Sexo).IsInEnum().WithMessage("Sexo inválido.");
        RuleFor(x => x.EstadoCivil).IsInEnum().WithMessage("Estado civil inválido.");
        RuleFor(x => x.ParentescoId).GreaterThan(0).WithMessage("Informe o parentesco.");
        RuleFor(x => x.NomeMae).MaximumLength(150);
        RuleFor(x => x.NomePai).MaximumLength(150);
        RuleFor(x => x.Escolaridade).MaximumLength(80);
        RuleFor(x => x.Ocupacao).MaximumLength(100);
        RuleFor(x => x.Telefone).MaximumLength(20);
        RuleFor(x => x.DescricaoDeficiencia)
            .NotEmpty().When(x => x.PessoaComDeficiencia)
            .WithMessage("Descreva a deficiência quando o membro for pessoa com deficiência.")
            .MaximumLength(255);

        RuleForEach(x => x.Documentos).SetValidator(new DocumentoRequestValidator());
        RuleForEach(x => x.Rendas).SetValidator(new RendaRequestValidator());
    }
}

public class AtualizarMembroRequestValidator : AbstractValidator<AtualizarMembroRequest>
{
    public AtualizarMembroRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().WithMessage("O nome do membro é obrigatório.").MaximumLength(150);
        RuleFor(x => x.DataNascimento)
            .NotEmpty().WithMessage("A data de nascimento é obrigatória.")
            .LessThanOrEqualTo(DateTime.Today).WithMessage("A data de nascimento não pode ser futura.");
        RuleFor(x => x.Sexo).IsInEnum().WithMessage("Sexo inválido.");
        RuleFor(x => x.EstadoCivil).IsInEnum().WithMessage("Estado civil inválido.");
        RuleFor(x => x.ParentescoId).GreaterThan(0).WithMessage("Informe o parentesco.");
        RuleFor(x => x.NomeMae).MaximumLength(150);
        RuleFor(x => x.NomePai).MaximumLength(150);
        RuleFor(x => x.Escolaridade).MaximumLength(80);
        RuleFor(x => x.Ocupacao).MaximumLength(100);
        RuleFor(x => x.Telefone).MaximumLength(20);
        RuleFor(x => x.DescricaoDeficiencia)
            .NotEmpty().When(x => x.PessoaComDeficiencia)
            .WithMessage("Descreva a deficiência quando o membro for pessoa com deficiência.")
            .MaximumLength(255);

        RuleForEach(x => x.Documentos).SetValidator(new DocumentoRequestValidator());
        RuleForEach(x => x.Rendas).SetValidator(new RendaRequestValidator());
    }
}
