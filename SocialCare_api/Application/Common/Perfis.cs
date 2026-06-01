namespace SocialCare.Application.Common;

/// <summary>Nomes dos papéis usados nas claims do JWT e nos atributos [Authorize(Roles = ...)].</summary>
public static class Perfis
{
    public const string Administrador = "Administrador";
    public const string Gestor = "Gestor";
    public const string AssistenteSocial = "AssistenteSocial";

    /// <summary>Operação direta com famílias: assistente, gestor e admin.</summary>
    public const string Operacao = $"{AssistenteSocial},{Gestor},{Administrador}";

    /// <summary>Gestão de programas, benefícios e relatórios: gestor e admin.</summary>
    public const string Gestao = $"{Gestor},{Administrador}";
}
