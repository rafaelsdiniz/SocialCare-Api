using FluentValidation;
using SocialCare.Application.DTOs.Usuarios;

namespace SocialCare.Application.Validators.Usuarios;

public class CriarUsuarioRequestValidator : AbstractValidator<CriarUsuarioRequest>
{
    public CriarUsuarioRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().WithMessage("O nome é obrigatório.").MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("E-mail inválido.").MaximumLength(200);
        RuleFor(x => x.Login).NotEmpty().WithMessage("O login é obrigatório.").MaximumLength(50);
        RuleFor(x => x.Senha).NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(8).WithMessage("A senha deve ter ao menos 8 caracteres.");
        RuleFor(x => x.PerfilIds).NotEmpty().WithMessage("Informe ao menos um perfil.");
    }
}

public class AtualizarUsuarioRequestValidator : AbstractValidator<AtualizarUsuarioRequest>
{
    public AtualizarUsuarioRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().WithMessage("O nome é obrigatório.").MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("E-mail inválido.").MaximumLength(200);
        RuleFor(x => x.PerfilIds).NotEmpty().WithMessage("Informe ao menos um perfil.");
    }
}

public class AlterarSenhaRequestValidator : AbstractValidator<AlterarSenhaRequest>
{
    public AlterarSenhaRequestValidator()
    {
        RuleFor(x => x.NovaSenha).NotEmpty().WithMessage("A nova senha é obrigatória.")
            .MinimumLength(8).WithMessage("A senha deve ter ao menos 8 caracteres.");
    }
}

public class AtualizarMinhaContaRequestValidator : AbstractValidator<AtualizarMinhaContaRequest>
{
    public AtualizarMinhaContaRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().WithMessage("O nome é obrigatório.").MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("E-mail inválido.").MaximumLength(200);
    }
}

public class AlterarMinhaSenhaRequestValidator : AbstractValidator<AlterarMinhaSenhaRequest>
{
    public AlterarMinhaSenhaRequestValidator()
    {
        RuleFor(x => x.SenhaAtual).NotEmpty().WithMessage("Informe a senha atual.");
        RuleFor(x => x.NovaSenha).NotEmpty().WithMessage("A nova senha é obrigatória.")
            .MinimumLength(8).WithMessage("A senha deve ter ao menos 8 caracteres.");
    }
}
