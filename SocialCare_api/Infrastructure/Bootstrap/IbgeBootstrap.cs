using Microsoft.EntityFrameworkCore;
using SocialCare.Application.Interfaces;
using SocialCare.Domain.Entities;
using SocialCare.Infrastructure.Data;

namespace SocialCare.Infrastructure.Bootstrap;

public static class IbgeBootstrap
{
    public static async Task PopularEstadosEMunicipiosAsync(IServiceProvider sp, ILogger logger, CancellationToken ct = default)
    {
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var ibge = scope.ServiceProvider.GetRequiredService<IIbgeClient>();

        var jaTemEstados = await db.Estados.AnyAsync(ct);
        if (jaTemEstados)
        {
            logger.LogInformation("Estados já populados. Bootstrap IBGE ignorado.");
            return;
        }

        logger.LogInformation("Populando Estados a partir do IBGE...");
        var estados = await ibge.ListarEstadosAsync(ct);
        if (estados.Count == 0)
        {
            logger.LogWarning("IBGE não retornou estados. Bootstrap abortado.");
            return;
        }

        var entidades = estados
            .Select(e => new Estado
            {
                CodigoIbge = e.Id,
                Sigla = e.Sigla,
                Nome = e.Nome,
                Regiao = e.Regiao,
                CriadoEm = DateTime.UtcNow,
                Ativo = true
            })
            .ToList();
        await db.Estados.AddRangeAsync(entidades, ct);
        await db.SaveChangesAsync(ct);
        logger.LogInformation("Inseridos {N} estados.", entidades.Count);

        var totalMunicipios = 0;
        foreach (var estado in entidades)
        {
            var municipios = await ibge.ListarMunicipiosAsync(estado.Sigla, ct);
            if (municipios.Count == 0) continue;

            var municipiosEntidades = municipios.Select(m => new Municipio
            {
                CodigoIbge = m.Id,
                Nome = m.Nome,
                EstadoId = estado.Id,
                CriadoEm = DateTime.UtcNow,
                Ativo = true
            });

            await db.Municipios.AddRangeAsync(municipiosEntidades, ct);
            await db.SaveChangesAsync(ct);
            totalMunicipios += municipios.Count;
            logger.LogInformation("UF {Uf}: {Qtd} municípios.", estado.Sigla, municipios.Count);
        }

        logger.LogInformation("Bootstrap IBGE concluído. Total: {Total} municípios.", totalMunicipios);
    }
}
