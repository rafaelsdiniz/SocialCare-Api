namespace SocialCare.Domain.Entities;

public class UsuarioPerfil
{
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public int PerfilId { get; set; }
    public Perfil Perfil { get; set; } = null!;

    public DateTime AtribuidoEm { get; set; } = DateTime.UtcNow;
}
