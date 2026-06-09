using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialCare.Domain.Entities;
using SocialCare.Domain.Enums;
using SocialCare.Infrastructure.Data;

namespace SocialCare.Infrastructure.Bootstrap;

/// <summary>
/// Popula a base com dados de demonstração: programas, municípios do Tocantins,
/// usuários assistentes, instituições parceiras, famílias (com membros, endereços,
/// vulnerabilidades e benefícios) e atividades (visitas, atendimentos e encaminhamentos).
/// É aditivo e idempotente — completa o que falta sem duplicar.
/// Controlado por <c>Demo:PopularNaInicializacao</c>.
/// </summary>
public static class DemoDataBootstrap
{
    private static readonly DateTime Agora = DateTime.UtcNow;
    private static readonly Random Rnd = new(20260602);

    /// <summary>Quantidade-alvo de famílias na base (completa até atingir).</summary>
    private const int AlvoFamilias = 200;

    public static async Task PopularAsync(IServiceProvider sp, IConfiguration config, ILogger logger, CancellationToken ct = default)
    {
        if (!config.GetValue("Demo:PopularNaInicializacao", false))
        {
            logger.LogInformation("Seed de demonstração desativado (Demo:PopularNaInicializacao=false).");
            return;
        }

        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Usuario>>();

        await GarantirProgramasAsync(db, logger, ct);
        var municipioIds = await GarantirMunicipiosTocantinsAsync(db, logger, ct);
        var assistenteIds = await GarantirUsuariosAsync(db, hasher, logger, ct);
        var instituicaoIds = await GarantirInstituicoesAsync(db, logger, ct);
        await GarantirFamiliasAsync(db, municipioIds, logger, ct);
        await GarantirAtividadesAsync(db, assistenteIds, instituicaoIds, logger, ct);
        await GarantirCoordenadasAsync(db, logger, ct);
    }

    // ---- Coordenadas de demonstração (mapa) ----------------------------------

    /// <summary>Centro aproximado (lat, lon) dos municípios do Tocantins usados no seed.</summary>
    private static readonly Dictionary<int, (double Lat, double Lon)> CentrosMunicipios = new()
    {
        [1721000] = (-10.1840, -48.3336), // Palmas
        [1702109] = (-7.1916, -48.2072),  // Araguaína
        [1709500] = (-11.7295, -49.0686), // Gurupi
        [1718204] = (-10.7081, -48.4172), // Porto Nacional
        [1716109] = (-10.1755, -48.8825), // Paraíso do Tocantins
        [1705508] = (-8.0589, -48.4753),  // Colinas do Tocantins
        [1709302] = (-8.8341, -48.5108),  // Guaraí
        [1721208] = (-6.3266, -47.4170),  // Tocantinópolis
        [1707009] = (-11.6260, -46.8210), // Dianópolis
        [1718501] = (-8.9709, -48.1750),  // Pedro Afonso
    };

    /// <summary>
    /// Preenche lat/lng nulos com o centro do município + pequeno deslocamento aleatório (~3 km),
    /// para o mapa público exibir os dados de demonstração sem chamar o geocoder. Idempotente.
    /// </summary>
    private static async Task GarantirCoordenadasAsync(AppDbContext db, ILogger logger, CancellationToken ct)
    {
        var enderecos = await db.Enderecos
            .Where(e => e.Latitude == null || e.Longitude == null)
            .Select(e => new { e.Id, e.Municipio.CodigoIbge })
            .ToListAsync(ct);

        if (enderecos.Count == 0)
        {
            logger.LogInformation("Coordenadas: nada a preencher.");
            return;
        }

        var atualizados = 0;
        foreach (var e in enderecos)
        {
            if (!CentrosMunicipios.TryGetValue(e.CodigoIbge, out var centro)) continue;

            var lat = centro.Lat + (Rnd.NextDouble() - 0.5) * 0.06;
            var lon = centro.Lon + (Rnd.NextDouble() - 0.5) * 0.06;
            await db.Enderecos
                .Where(x => x.Id == e.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.Latitude, lat).SetProperty(x => x.Longitude, lon), ct);
            atualizados++;
        }

        logger.LogInformation("Coordenadas: {N} endereços geolocalizados (demo).", atualizados);
    }

    // ---- Programas sociais conhecidos ----------------------------------------

    private record ProgramaSeed(string Nome, string Orgao, string Descricao, string Requisitos, decimal? Valor);

    private static readonly ProgramaSeed[] ProgramasConhecidos =
    [
        new("Bolsa Família", "Ministério do Desenvolvimento e Assistência Social",
            "Transferência de renda para famílias em situação de pobreza e extrema pobreza, com foco em crianças, gestantes e adolescentes.",
            "Renda mensal por pessoa de até R$ 218 e inscrição atualizada no CadÚnico.", 600m),
        new("Auxílio Gás dos Brasileiros", "Ministério do Desenvolvimento e Assistência Social",
            "Auxílio bimestral para a compra do botijão de gás de cozinha (GLP) por famílias de baixa renda.",
            "Inscrição no CadÚnico e renda familiar por pessoa de até meio salário mínimo.", 102m),
        new("BPC - Benefício de Prestação Continuada", "INSS / MDS",
            "Garante um salário mínimo mensal a idosos com 65 anos ou mais e a pessoas com deficiência sem meios de prover o próprio sustento.",
            "Renda familiar por pessoa inferior a 1/4 do salário mínimo e inscrição no CadÚnico.", 1412m),
        new("Programa Leite das Crianças", "Secretaria de Estado da Cidadania e Justiça - TO",
            "Distribuição gratuita de leite a crianças de 6 meses a 6 anos em situação de vulnerabilidade no Tocantins.",
            "Crianças de 6 meses a 6 anos cadastradas e em risco nutricional.", null),
        new("Tarifa Social de Energia Elétrica", "ANEEL",
            "Desconto na conta de energia elétrica para famílias de baixa renda inscritas no CadÚnico.",
            "Inscrição no CadÚnico com renda de até meio salário mínimo por pessoa ou beneficiário do BPC.", null),
        new("Pé-de-Meia", "Ministério da Educação",
            "Incentivo financeiro-educacional (poupança) para estudantes de baixa renda matriculados no ensino médio público.",
            "Estudante do ensino médio da rede pública, membro de família inscrita no CadÚnico.", 200m),
        new("Farmácia Popular", "Ministério da Saúde",
            "Acesso gratuito ou com desconto a medicamentos para hipertensão, diabetes, asma e outros, em farmácias credenciadas.",
            "Apresentar prescrição médica e documento com CPF.", null),
        new("Minha Casa, Minha Vida", "Ministério das Cidades",
            "Facilita o acesso à casa própria para famílias de baixa renda com subsídios e financiamento facilitado.",
            "Renda familiar dentro das faixas do programa e não possuir imóvel.", null),
        new("Programa Nacional de Alimentação Escolar (PNAE)", "FNDE / Ministério da Educação",
            "Oferta de alimentação escolar gratuita a estudantes da educação básica da rede pública.",
            "Estar matriculado na rede pública de educação básica.", null),
        new("Programa Cisternas", "Ministério do Desenvolvimento e Assistência Social",
            "Construção de cisternas para acesso à água potável em regiões de seca e baixa disponibilidade hídrica.",
            "Famílias rurais de baixa renda em áreas de escassez de água, inscritas no CadÚnico.", null),
        new("Dignidade Menstrual", "Ministério da Saúde",
            "Distribuição gratuita de absorventes higiênicos a pessoas em situação de vulnerabilidade.",
            "Estudantes de baixa renda, pessoas em situação de rua ou inscritas no CadÚnico.", null),
        new("Auxílio Natalidade", "Secretaria de Estado da Cidadania e Justiça - TO",
            "Apoio financeiro e enxoval para gestantes e recém-nascidos de famílias em vulnerabilidade no Tocantins.",
            "Gestantes inscritas no CadÚnico em acompanhamento pela rede socioassistencial.", null),
    ];

    private static async Task GarantirProgramasAsync(AppDbContext db, ILogger logger, CancellationToken ct)
    {
        var existentes = await db.ProgramasSociais.ToListAsync(ct);
        var porNome = existentes.ToDictionary(p => p.Nome, StringComparer.OrdinalIgnoreCase);
        var novos = 0;

        foreach (var seed in ProgramasConhecidos)
        {
            if (porNome.TryGetValue(seed.Nome, out var existente))
            {
                existente.Descricao ??= seed.Descricao;
                existente.Requisitos ??= seed.Requisitos;
                existente.ValorPadrao ??= seed.Valor;
                continue;
            }

            db.ProgramasSociais.Add(new ProgramaSocial
            {
                Nome = seed.Nome,
                OrgaoResponsavel = seed.Orgao,
                Descricao = seed.Descricao,
                Requisitos = seed.Requisitos,
                ValorPadrao = seed.Valor,
                CriadoEm = Agora,
                Ativo = true,
            });
            novos++;
        }

        await db.SaveChangesAsync(ct);
        logger.LogInformation("Programas: {Novos} inseridos, {Total} no total.", novos, existentes.Count + novos);
    }

    // ---- Municípios do Tocantins ---------------------------------------------

    private static readonly (int CodigoIbge, string Nome)[] MunicipiosTo =
    [
        (1721000, "Palmas"),
        (1702109, "Araguaína"),
        (1709500, "Gurupi"),
        (1718204, "Porto Nacional"),
        (1716109, "Paraíso do Tocantins"),
        (1705508, "Colinas do Tocantins"),
        (1709302, "Guaraí"),
        (1721208, "Tocantinópolis"),
        (1707009, "Dianópolis"),
        (1718501, "Pedro Afonso"),
    ];

    private static async Task<List<int>> GarantirMunicipiosTocantinsAsync(AppDbContext db, ILogger logger, CancellationToken ct)
    {
        var existentes = await db.Municipios.Select(m => m.Id).ToListAsync(ct);
        if (existentes.Count > 0)
        {
            logger.LogInformation("Municípios já existem ({N}). Usando os cadastrados.", existentes.Count);
            return existentes;
        }

        var estado = await db.Estados.FirstOrDefaultAsync(e => e.Sigla == "TO", ct);
        if (estado is null)
        {
            estado = new Estado
            {
                CodigoIbge = 17,
                Sigla = "TO",
                Nome = "Tocantins",
                Regiao = "Norte",
                CriadoEm = Agora,
                Ativo = true,
            };
            db.Estados.Add(estado);
            await db.SaveChangesAsync(ct);
        }

        var municipios = MunicipiosTo.Select(m => new Municipio
        {
            CodigoIbge = m.CodigoIbge,
            Nome = m.Nome,
            EstadoId = estado.Id,
            CriadoEm = Agora,
            Ativo = true,
        }).ToList();

        db.Municipios.AddRange(municipios);
        await db.SaveChangesAsync(ct);
        logger.LogInformation("Inseridos {N} municípios do Tocantins.", municipios.Count);

        return municipios.Select(m => m.Id).ToList();
    }

    // ---- Usuários assistentes -------------------------------------------------

    private static readonly (string Nome, string Login)[] AssistentesDemo =
    [
        ("Ana Paula Ribeiro", "ana.ribeiro"),
        ("Carlos Eduardo Lima", "carlos.lima"),
        ("Mariana Souza Costa", "mariana.costa"),
        ("Roberto Alves Pereira", "roberto.pereira"),
        ("Juliana Martins Dias", "juliana.dias"),
    ];

    private static async Task<List<int>> GarantirUsuariosAsync(AppDbContext db, IPasswordHasher<Usuario> hasher, ILogger logger, CancellationToken ct)
    {
        var perfilAssistente = await db.Perfis.FirstOrDefaultAsync(p => p.Nome == "AssistenteSocial", ct);
        var novos = 0;

        foreach (var (nome, login) in AssistentesDemo)
        {
            if (await db.Usuarios.AnyAsync(u => u.Login == login, ct)) continue;

            var usuario = new Usuario
            {
                Nome = nome,
                Login = login,
                Email = $"{login}@socialcare.to.gov.br",
                CriadoEm = Agora,
                Ativo = true,
            };
            usuario.SenhaHash = hasher.HashPassword(usuario, "ChangeMe@123");
            db.Usuarios.Add(usuario);
            await db.SaveChangesAsync(ct);
            novos++;

            if (perfilAssistente is not null)
            {
                db.UsuarioPerfis.Add(new UsuarioPerfil { UsuarioId = usuario.Id, PerfilId = perfilAssistente.Id, AtribuidoEm = Agora });
                await db.SaveChangesAsync(ct);
            }
        }

        logger.LogInformation("Usuários assistentes: {Novos} criados.", novos);

        // Todos os usuários podem atuar como responsáveis nas atividades de demonstração.
        return await db.Usuarios.Select(u => u.Id).ToListAsync(ct);
    }

    // ---- Instituições parceiras ----------------------------------------------

    private static readonly (string Nome, string Area)[] InstituicoesDemo =
    [
        ("CRAS Centro - Palmas", "Assistência Social"),
        ("CREAS Palmas", "Proteção Social Especial"),
        ("Conselho Tutelar de Palmas", "Direitos da Criança e do Adolescente"),
        ("Hospital Geral de Palmas", "Saúde"),
        ("SENAI Tocantins", "Educação Profissional"),
        ("Defensoria Pública do Tocantins", "Assistência Jurídica"),
        ("CAPS - Centro de Atenção Psicossocial", "Saúde Mental"),
        ("Casa de Acolhimento Esperança", "Acolhimento Institucional"),
    ];

    private static async Task<List<int>> GarantirInstituicoesAsync(AppDbContext db, ILogger logger, CancellationToken ct)
    {
        var existentes = await db.InstituicoesParceiras.Select(i => i.Id).ToListAsync(ct);
        if (existentes.Count > 0)
        {
            logger.LogInformation("Instituições já existem ({N}).", existentes.Count);
            return existentes;
        }

        var instituicoes = InstituicoesDemo.Select((dados, i) => new InstituicaoParceira
        {
            Nome = dados.Nome,
            AreaAtuacao = dados.Area,
            Cnpj = $"{10 + i:D2}.{100 + i:D3}.{200 + i:D3}/0001-{50 + i:D2}",
            Telefone = $"(63) 3{200 + i:D3}-{1000 + i:D4}",
            Email = $"contato{i + 1}@parceiro.to.gov.br",
            ResponsavelContato = "Coordenação",
            EnderecoCompleto = "Palmas - TO",
            CriadoEm = Agora,
            Ativo = true,
        }).ToList();

        db.InstituicoesParceiras.AddRange(instituicoes);
        await db.SaveChangesAsync(ct);
        logger.LogInformation("Inseridas {N} instituições parceiras.", instituicoes.Count);

        return instituicoes.Select(i => i.Id).ToList();
    }

    // ---- Famílias, membros, endereços e benefícios ---------------------------

    private static readonly string[] Sobrenomes =
    [
        "Silva", "Santos", "Oliveira", "Souza", "Lima", "Pereira", "Ferreira", "Costa", "Rodrigues",
        "Almeida", "Nascimento", "Carvalho", "Araújo", "Ribeiro", "Gomes", "Martins", "Barbosa",
        "Rocha", "Dias", "Moreira", "Cardoso", "Teixeira", "Correia", "Mendes", "Cavalcante",
    ];
    private static readonly string[] NomesF =
    [
        "Maria", "Ana", "Francisca", "Antônia", "Joana", "Luiza", "Rita", "Cláudia", "Sandra",
        "Patrícia", "Adriana", "Fernanda", "Juliana", "Marcia", "Vanessa", "Camila", "Raimunda",
    ];
    private static readonly string[] NomesM =
    [
        "José", "João", "Antônio", "Francisco", "Carlos", "Paulo", "Pedro", "Lucas", "Luiz",
        "Marcos", "Raimundo", "Sebastião", "Rafael", "Gabriel", "Daniel", "Manoel", "Roberto",
    ];
    private static readonly string[] Bairros =
    [
        "Centro", "Jardim Aureny", "Taquaralto", "Setor Sul", "Vila Nova", "Setor Norte",
        "Jardim Paulista", "Morada do Sol", "Setor Industrial", "Bela Vista",
    ];
    private static readonly string[] Logradouros =
    [
        "Rua das Flores", "Avenida JK", "Rua 7 de Setembro", "Quadra 12 Alameda 4",
        "Rua dos Buritis", "Avenida Tocantins", "Rua do Cerrado", "Quadra 305 Sul",
    ];
    private static readonly string[] Ocupacoes =
    [
        "Diarista", "Pedreiro", "Agricultor(a)", "Auxiliar de serviços gerais", "Costureira",
        "Vendedor(a) ambulante", "Desempregado(a)", "Aposentado(a)", "Cuidador(a)", "Motorista",
    ];

    private static async Task GarantirFamiliasAsync(AppDbContext db, List<int> municipioIds, ILogger logger, CancellationToken ct)
    {
        if (municipioIds.Count == 0)
        {
            logger.LogWarning("Sem municípios cadastrados — não é possível gerar famílias.");
            return;
        }

        var existentes = await db.Familias.CountAsync(ct);
        if (existentes >= AlvoFamilias)
        {
            logger.LogInformation("Já há {N} famílias (alvo {Alvo}). Nada a adicionar.", existentes, AlvoFamilias);
            return;
        }

        const int parentescoResponsavel = 1;
        const int parentescoConjuge = 2;
        const int parentescoFilho = 3;

        var programas = await db.ProgramasSociais
            .Where(p => p.Ativo)
            .Select(p => new { p.Id, p.ValorPadrao })
            .ToListAsync(ct);
        var vulnerabilidadeIds = await db.Vulnerabilidades.Select(v => v.Id).ToListAsync(ct);

        var statusPossiveis = new[]
        {
            StatusFamilia.Ativa, StatusFamilia.Ativa, StatusFamilia.Ativa,
            StatusFamilia.EmAcompanhamento, StatusFamilia.EmAcompanhamento, StatusFamilia.Inativa,
        };

        var criadas = 0;
        for (var seq = existentes + 1; seq <= AlvoFamilias; seq++)
        {
            var responsavelF = Rnd.NextDouble() < 0.7;
            var sobrenome = Escolher(Sobrenomes);
            var nomeResp = $"{(responsavelF ? Escolher(NomesF) : Escolher(NomesM))} {Escolher(Sobrenomes)} {sobrenome}";

            var familia = new Familia
            {
                CodigoFamiliar = $"TO{20260000 + seq:D8}",
                NomeResponsavel = nomeResp,
                Status = Escolher(statusPossiveis),
                DataCadastro = Agora.AddDays(-Rnd.Next(15, 720)),
                CriadoEm = Agora,
                Ativo = true,
            };

            var membros = new List<Membro>
            {
                NovoMembro(nomeResp, responsavelF ? Sexo.Feminino : Sexo.Masculino, parentescoResponsavel, 25, 60),
            };
            var resp = membros[0];

            if (Rnd.NextDouble() < 0.45)
            {
                var sexoConj = responsavelF ? Sexo.Masculino : Sexo.Feminino;
                membros.Add(NovoMembro($"{(sexoConj == Sexo.Feminino ? Escolher(NomesF) : Escolher(NomesM))} {sobrenome}", sexoConj, parentescoConjuge, 25, 60, EstadoCivil.Casado));
            }

            var qtdFilhos = Rnd.Next(0, 4);
            for (var f = 0; f < qtdFilhos; f++)
            {
                var sexoFilho = Rnd.NextDouble() < 0.5 ? Sexo.Masculino : Sexo.Feminino;
                membros.Add(NovoMembro($"{(sexoFilho == Sexo.Feminino ? Escolher(NomesF) : Escolher(NomesM))} {sobrenome}", sexoFilho, parentescoFilho, 1, 17, EstadoCivil.Solteiro));
            }

            familia.QuantidadeMembros = membros.Count;
            foreach (var m in membros) familia.Membros.Add(m);

            var renda = Rnd.Next(0, 7) == 0 ? 0m : Rnd.Next(300, 2600);
            familia.RendaTotalMensal = renda;
            familia.RendaPerCapita = Math.Round(renda / membros.Count, 2);

            familia.Endereco = new Endereco
            {
                Cep = $"77{Rnd.Next(0, 1000):D3}-{Rnd.Next(0, 1000):D3}",
                Logradouro = Escolher(Logradouros),
                Numero = Rnd.Next(1, 1500).ToString(),
                Bairro = Escolher(Bairros),
                MunicipioId = Escolher(municipioIds),
                CriadoEm = Agora,
                Ativo = true,
            };

            if (vulnerabilidadeIds.Count > 0)
            {
                var escolhidas = vulnerabilidadeIds.OrderBy(_ => Rnd.Next()).Take(Rnd.Next(0, 3));
                foreach (var vid in escolhidas)
                    familia.Vulnerabilidades.Add(new FamiliaVulnerabilidade { VulnerabilidadeId = vid, IdentificadaEm = familia.DataCadastro });
            }

            if (programas.Count > 0 && familia.Status != StatusFamilia.Inativa)
            {
                var progEscolhidos = programas.OrderBy(_ => Rnd.Next()).Take(Rnd.Next(1, 3));
                foreach (var prog in progEscolhidos)
                {
                    familia.Beneficios.Add(new Beneficio
                    {
                        ProgramaSocialId = prog.Id,
                        Valor = prog.ValorPadrao ?? Rnd.Next(80, 700),
                        Status = StatusBeneficio.Ativo,
                        DataInicio = familia.DataCadastro.AddDays(Rnd.Next(0, 60)),
                        CriadoEm = Agora,
                        Ativo = true,
                    });
                }
            }

            db.Familias.Add(familia);
            await db.SaveChangesAsync(ct);

            familia.MembroResponsavelId = resp.Id;
            await db.SaveChangesAsync(ct);
            criadas++;
        }

        logger.LogInformation("Famílias: {Criadas} criadas (total agora {Total}).", criadas, existentes + criadas);
    }

    // ---- Atividades: visitas, atendimentos e encaminhamentos -----------------

    private static readonly string[] MotivosVisita =
    [
        "Acompanhamento familiar", "Verificação das condições de moradia", "Atualização cadastral",
        "Busca ativa", "Orientação sobre benefícios", "Avaliação socioeconômica",
    ];
    private static readonly string[] MotivosAtendimento =
    [
        "Solicitação de benefício", "Orientação social", "Encaminhamento para a rede",
        "Atualização do CadÚnico", "Situação de vulnerabilidade", "Acompanhamento de medida protetiva",
    ];
    private static readonly string[] MotivosEncaminhamento =
    [
        "Atendimento psicológico", "Inserção em curso profissionalizante", "Atendimento de saúde",
        "Apoio jurídico", "Acolhimento institucional", "Acompanhamento nutricional",
    ];

    private static async Task GarantirAtividadesAsync(AppDbContext db, List<int> assistenteIds, List<int> instituicaoIds, ILogger logger, CancellationToken ct)
    {
        if (assistenteIds.Count == 0)
        {
            logger.LogWarning("Sem usuários para responsabilizar pelas atividades. Etapa ignorada.");
            return;
        }

        // Famílias que ainda não têm nenhuma visita recebem o pacote de atividades.
        var familias = await db.Familias
            .Where(f => !f.Visitas.Any())
            .Select(f => new { f.Id, f.DataCadastro, MembroIds = f.Membros.Select(m => m.Id).ToList() })
            .ToListAsync(ct);

        if (familias.Count == 0)
        {
            logger.LogInformation("Todas as famílias já possuem atividades. Nada a fazer.");
            return;
        }

        var totalVisitas = 0;
        var totalAtendimentos = 0;
        var totalEncaminhamentos = 0;

        foreach (var f in familias)
        {
            // Visitas (1 a 3) — ao menos uma recente para alimentar o indicador de 30 dias.
            var qtdVisitas = Rnd.Next(1, 4);
            for (var k = 0; k < qtdVisitas; k++)
            {
                var recente = k == 0; // a primeira é recente
                var agendada = recente ? Agora.AddDays(-Rnd.Next(0, 28)) : Agora.AddDays(-Rnd.Next(30, 200));
                var jaPassou = agendada < Agora;
                var realizada = jaPassou && Rnd.NextDouble() < 0.7;
                db.Visitas.Add(new Visita
                {
                    FamiliaId = f.Id,
                    AssistenteResponsavelId = Escolher(assistenteIds),
                    DataAgendada = agendada,
                    DataRealizacao = realizada ? agendada : null,
                    Tipo = (TipoVisita)Rnd.Next(1, 5),
                    Status = realizada ? StatusVisita.Realizada : jaPassou ? StatusVisita.NaoRealizada : StatusVisita.Agendada,
                    Motivo = Escolher(MotivosVisita),
                    Observacoes = realizada ? "Família acompanhada conforme planejado." : null,
                    CriadoEm = Agora,
                    Ativo = true,
                });
                totalVisitas++;
            }

            // Atendimentos (1 a 2)
            var qtdAtend = Rnd.Next(1, 3);
            for (var k = 0; k < qtdAtend; k++)
            {
                var status = (StatusAtendimento)Rnd.Next(1, 5);
                db.Atendimentos.Add(new Atendimento
                {
                    FamiliaId = f.Id,
                    AssistenteResponsavelId = Escolher(assistenteIds),
                    DataAtendimento = Agora.AddDays(-Rnd.Next(0, 180)),
                    Motivo = Escolher(MotivosAtendimento),
                    Demanda = "Demanda registrada no acolhimento.",
                    Parecer = status == StatusAtendimento.Concluido ? "Atendimento concluído com encaminhamentos." : null,
                    Remoto = Rnd.NextDouble() < 0.3,
                    Status = status,
                    CriadoEm = Agora,
                    Ativo = true,
                });
                totalAtendimentos++;
            }

            // Encaminhamentos (0 a 2)
            if (instituicaoIds.Count > 0)
            {
                var qtdEnc = Rnd.Next(0, 3);
                for (var k = 0; k < qtdEnc; k++)
                {
                    var status = (StatusEncaminhamento)Rnd.Next(1, 6);
                    var concluido = status is StatusEncaminhamento.Concluido or StatusEncaminhamento.Recusado;
                    var dataEnc = Agora.AddDays(-Rnd.Next(0, 150));
                    db.Encaminhamentos.Add(new Encaminhamento
                    {
                        FamiliaId = f.Id,
                        InstituicaoParceiraId = Escolher(instituicaoIds),
                        MembroId = f.MembroIds.Count > 0 && Rnd.NextDouble() < 0.6 ? Escolher(f.MembroIds) : null,
                        Motivo = Escolher(MotivosEncaminhamento),
                        Demanda = "Encaminhamento à rede socioassistencial.",
                        Status = status,
                        DataEncaminhamento = dataEnc,
                        DataRetorno = concluido ? dataEnc.AddDays(Rnd.Next(5, 40)) : null,
                        Retorno = concluido ? "Retorno registrado pela instituição parceira." : null,
                        CriadoEm = Agora,
                        Ativo = true,
                    });
                    totalEncaminhamentos++;
                }
            }

            await db.SaveChangesAsync(ct);
        }

        logger.LogInformation(
            "Atividades geradas para {Fam} famílias: {V} visitas, {A} atendimentos, {E} encaminhamentos.",
            familias.Count, totalVisitas, totalAtendimentos, totalEncaminhamentos);
    }

    private static Membro NovoMembro(string nome, Sexo sexo, int parentescoId, int idadeMin, int idadeMax, EstadoCivil estadoCivil = EstadoCivil.Solteiro)
    {
        var idade = Rnd.Next(idadeMin, idadeMax + 1);
        return new Membro
        {
            Nome = nome,
            Sexo = sexo,
            ParentescoId = parentescoId,
            DataNascimento = Agora.AddYears(-idade).AddDays(-Rnd.Next(0, 365)),
            EstadoCivil = estadoCivil,
            Ocupacao = parentescoId == 3 ? "Estudante" : Escolher(Ocupacoes),
            CriadoEm = Agora,
            Ativo = true,
        };
    }

    private static T Escolher<T>(IReadOnlyList<T> itens) => itens[Rnd.Next(itens.Count)];
}
