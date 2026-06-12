namespace SocialCare.Application.Common;

/// <summary>Chaves do cache em memória compartilhadas entre serviços e endpoints públicos.</summary>
public static class CacheKeys
{
    /// <summary>Lista pública de programas (vitrine). Invalidada ao criar/editar/inativar um programa.</summary>
    public const string ProgramasPublico = "publico:programas";
}
