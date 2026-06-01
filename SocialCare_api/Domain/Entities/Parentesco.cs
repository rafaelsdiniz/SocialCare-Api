namespace SocialCare.Domain.Entities;

public class Parentesco : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }

    public ICollection<Membro> Membros { get; set; } = new List<Membro>();
}
