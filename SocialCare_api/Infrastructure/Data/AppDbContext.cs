using Microsoft.EntityFrameworkCore;
using SocialCare.Domain.Entities;

namespace SocialCare.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Perfil> Perfis => Set<Perfil>();
    public DbSet<UsuarioPerfil> UsuarioPerfis => Set<UsuarioPerfil>();
    public DbSet<LogAuditoria> LogsAuditoria => Set<LogAuditoria>();

    public DbSet<Familia> Familias => Set<Familia>();
    public DbSet<Membro> Membros => Set<Membro>();
    public DbSet<Parentesco> Parentescos => Set<Parentesco>();
    public DbSet<Documento> Documentos => Set<Documento>();
    public DbSet<TipoDocumento> TiposDocumento => Set<TipoDocumento>();

    public DbSet<Endereco> Enderecos => Set<Endereco>();
    public DbSet<Municipio> Municipios => Set<Municipio>();
    public DbSet<Estado> Estados => Set<Estado>();

    public DbSet<Renda> Rendas => Set<Renda>();
    public DbSet<TipoRenda> TiposRenda => Set<TipoRenda>();
    public DbSet<Vulnerabilidade> Vulnerabilidades => Set<Vulnerabilidade>();
    public DbSet<FamiliaVulnerabilidade> FamiliaVulnerabilidades => Set<FamiliaVulnerabilidade>();

    public DbSet<ProgramaSocial> ProgramasSociais => Set<ProgramaSocial>();
    public DbSet<Beneficio> Beneficios => Set<Beneficio>();
    public DbSet<InstituicaoParceira> InstituicoesParceiras => Set<InstituicaoParceira>();

    public DbSet<Visita> Visitas => Set<Visita>();
    public DbSet<Atendimento> Atendimentos => Set<Atendimento>();
    public DbSet<Encaminhamento> Encaminhamentos => Set<Encaminhamento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        DataSeeder.Seed(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AtualizarTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void AtualizarTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.AtualizadoEm = DateTime.UtcNow;
            }
        }
    }
}
