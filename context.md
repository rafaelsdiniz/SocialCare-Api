# SocialCare — Contexto do Projeto

> Sistema de Gestão de Assistência Social
> Avaliação Final — Tópicos III
> Aluno: Rafael Diniz

---

## 1. Visão Geral

O **SocialCare** é uma plataforma web + API para gestão de assistência social em
órgãos públicos (ex.: CRAS/CREAS) ou ONGs. Permite o cadastro de famílias em
situação de vulnerabilidade, controle de benefícios concedidos, agendamento e
registro de visitas domiciliares, gestão de programas sociais, atendimentos,
encaminhamentos para instituições parceiras e emissão de relatórios gerenciais.

A API também é exposta para parceiros (BIs, prefeituras, aplicativos), com
endpoints públicos (consultas agregadas/anônimas) e autenticados (operação).

### Domínio de negócio

- **Família**: núcleo do sistema. Contém membros, endereço, rendas, situações
  de vulnerabilidade, benefícios e histórico de atendimentos/visitas.
- **Membro**: pessoa física vinculada a uma família, com documentos e renda
  individual.
- **Programa Social**: iniciativa governamental ou própria (Bolsa Família,
  CadÚnico, Auxílio Gás, Programa Leite, etc.).
- **Benefício**: instância de um programa concedido a uma família, com data de
  início, validade e valor.
- **Visita**: registro de visita domiciliar feita por um Assistente Social.
- **Atendimento**: atendimento presencial/remoto registrado no CRAS.
- **Encaminhamento**: redirecionamento de família/membro para instituição
  parceira (saúde, educação, jurídico, etc.).

---

## 2. Papéis e Permissões

| Papel | Responsabilidades |
|---|---|
| **Administrador** | Gerencia usuários, perfis, parâmetros do sistema, audita logs. |
| **Gestor** | Visualiza relatórios, aprova benefícios, gerencia programas e instituições parceiras. |
| **Assistente Social** | Cadastra famílias/membros, agenda e registra visitas, abre atendimentos, lança encaminhamentos. |

---

## 3. APIs de Terceiros Consumidas

| API | Uso |
|---|---|
| **ViaCEP** (`viacep.com.br`) | Autopreenchimento de endereço a partir do CEP. |
| **IBGE Localidades** (`servicodados.ibge.gov.br/api/v1/localidades`) | População inicial das tabelas `Estado` e `Município`; validação de UF/cidade. |
| **CadÚnico — Dados Abertos** (`aplicacoes.mds.gov.br/sagi/servicos`) | Consulta complementar de famílias/benefícios públicos quando disponível. |

---

## 4. Arquitetura da Solução

Arquitetura em camadas (Clean Architecture *light*), separando domínio,
aplicação, infraestrutura e apresentação. Mantém o projeto testável e respeita
o controle de versão de banco (EF Core Migrations + Seed).

```
SocialCare_api/                           (solução)
├── SocialCare.API/                       ASP.NET Core 10 Web API
│   ├── Controllers/                      Endpoints REST
│   ├── Middlewares/                      ExceptionHandling, AuditLog
│   ├── Extensions/                       DI, Swagger, JWT, CORS
│   ├── appsettings.json
│   └── Program.cs
│
├── SocialCare.Domain/                    Núcleo do negócio (sem dependências)
│   ├── Entities/                         POCOs (21 entidades)
│   ├── Enums/                            StatusFamilia, TipoVisita, etc.
│   └── Interfaces/                       IRepository, IUnitOfWork
│
├── SocialCare.Application/               Casos de uso e regras de aplicação
│   ├── DTOs/                             Request/Response models
│   ├── Services/                         FamiliaService, BeneficioService, ...
│   ├── Validators/                       FluentValidation
│   ├── Mappings/                         AutoMapper profiles
│   └── Interfaces/                       IFamiliaService, IViaCepClient
│
├── SocialCare.Infrastructure/            Persistência e integrações
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   ├── Configurations/               IEntityTypeConfiguration<T>
│   │   ├── Migrations/                   EF Core Migrations
│   │   └── Seed/                         Seed inicial (perfis, UF, programas)
│   ├── Repositories/
│   ├── ExternalApis/                     ViaCepClient, IbgeClient, CadUnicoClient
│   └── Identity/                         JWT, password hashing
│
├── SocialCare.Web/                       Front-end (Razor Pages ou Blazor Server)
│   ├── Pages/ ou Components/
│   ├── wwwroot/                          CSS/JS (Bootstrap + tema próprio)
│   └── Shared/
│
└── SocialCare.Tests/                     xUnit + bogus + WebApplicationFactory
```

> **Front-end**: Razor Pages com Bootstrap 5 customizado (identidade visual
> própria — paleta institucional, logotipo, tipografia consistente). Caso o
> tempo permita, migrar telas críticas para Blazor Server.

---

## 5. Entidades (21 — mínimo exigido: 18)

### Bloco A — Identidade e Auditoria (4)

| # | Entidade | Descrição |
|---|---|---|
| 1 | **Usuario** | Usuário do sistema (login, e-mail, senha-hash, ativo). |
| 2 | **Perfil** | Papel (Administrador, Gestor, AssistenteSocial). |
| 3 | **UsuarioPerfil** | Tabela associativa N:N entre Usuário e Perfil. |
| 4 | **LogAuditoria** | Trilha de auditoria (quem, o quê, quando, dados antes/depois). |

### Bloco B — Família e Pessoas (5)

| # | Entidade | Descrição |
|---|---|---|
| 5 | **Familia** | Núcleo familiar. Possui um responsável (Membro), endereço, renda total computada e status. |
| 6 | **Membro** | Pessoa física integrante de uma família. |
| 7 | **Parentesco** | Tipo de vínculo (Responsável, Cônjuge, Filho(a), Avó, Outro). |
| 8 | **Documento** | Documento associado a um membro (CPF, RG, NIS, CertidãoNasc). |
| 9 | **TipoDocumento** | Catálogo de tipos de documento. |

### Bloco C — Localização (3)

| # | Entidade | Descrição |
|---|---|---|
| 10 | **Endereco** | Endereço da família (CEP, logradouro, número, bairro). Populado via ViaCEP. |
| 11 | **Municipio** | Município (cód. IBGE, nome). Popular via API do IBGE. |
| 12 | **Estado** | UF (cód. IBGE, sigla, nome). Popular via API do IBGE. |

### Bloco D — Renda e Vulnerabilidade (3)

| # | Entidade | Descrição |
|---|---|---|
| 13 | **Renda** | Renda declarada por um membro (valor, fonte, mês/ano). |
| 14 | **TipoRenda** | Categoria da renda (Salário CLT, Autônomo, Aposentadoria, Bolsa, Pensão). |
| 15 | **Vulnerabilidade** | Situação de vulnerabilidade da família (insegurança alimentar, moradia precária, violência doméstica, etc.). N:N com Família via tabela `FamiliaVulnerabilidade` — alternativamente, modelar como entidade própria. |

### Bloco E — Programas e Benefícios (3)

| # | Entidade | Descrição |
|---|---|---|
| 16 | **ProgramaSocial** | Programa cadastrado (nome, descrição, requisitos, órgão responsável). |
| 17 | **Beneficio** | Concessão de um programa a uma família (data início, fim, valor, status). |
| 18 | **InstituicaoParceira** | Órgão/ONG parceira para encaminhamentos (CNPJ, área de atuação, contato). |

### Bloco F — Operação Social (3)

| # | Entidade | Descrição |
|---|---|---|
| 19 | **Visita** | Visita domiciliar (data, assistente, observações, status). |
| 20 | **Atendimento** | Atendimento presencial/remoto (data, motivo, parecer). |
| 21 | **Encaminhamento** | Encaminhamento de família para instituição parceira (motivo, data, retorno). |

---

## 6. Relacionamentos (resumo)

```
Usuario *──* Perfil                    (via UsuarioPerfil)
Usuario  1──* LogAuditoria
Usuario  1──* Visita        (assistenteResponsavel)
Usuario  1──* Atendimento   (assistenteResponsavel)

Familia  1──* Membro
Familia  1──1 Endereco
Familia  *──* Vulnerabilidade
Familia  1──* Beneficio
Familia  1──* Visita
Familia  1──* Atendimento
Familia  1──* Encaminhamento

Membro   *──1 Parentesco
Membro   1──* Documento
Membro   *──1 TipoDocumento (via Documento)
Membro   1──* Renda
Renda    *──1 TipoRenda

Endereco *──1 Municipio
Municipio *──1 Estado

Beneficio       *──1 ProgramaSocial
Encaminhamento  *──1 InstituicaoParceira
```

---

## 7. Endpoints da API (mínimo 3 públicos + 3 autenticados)

### Públicos (sem JWT)

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/publico/estados` | Lista UFs (cache 24h). |
| GET | `/api/publico/municipios/{ufSigla}` | Lista municípios da UF. |
| GET | `/api/publico/programas` | Lista programas sociais ativos (vitrine). |
| GET | `/api/publico/indicadores` | Estatísticas anonimizadas (nº famílias/UF, etc.). |
| GET | `/api/publico/cep/{cep}` | Proxy ViaCEP. |

### Autenticados (JWT)

| Método | Rota | Perfil mínimo |
|---|---|---|
| POST | `/api/auth/login` | — |
| GET/POST/PUT/DELETE | `/api/familias` | AssistenteSocial |
| GET/POST/PUT/DELETE | `/api/familias/{id}/membros` | AssistenteSocial |
| GET/POST/PUT | `/api/visitas` | AssistenteSocial |
| GET/POST/PUT | `/api/atendimentos` | AssistenteSocial |
| GET/POST/PUT | `/api/beneficios` | Gestor |
| GET/POST/PUT | `/api/programas` | Gestor |
| GET/POST/PUT | `/api/instituicoes-parceiras` | Gestor |
| GET/POST/PUT/DELETE | `/api/usuarios` | Administrador |
| GET | `/api/relatorios/familias-por-vulnerabilidade` | Gestor |
| GET | `/api/relatorios/beneficios-por-programa` | Gestor |
| GET | `/api/relatorios/visitas-por-assistente` | Gestor |
| GET | `/api/auditoria` | Administrador |

---

## 8. Tecnologias

| Camada | Stack |
|---|---|
| Back-end | ASP.NET Core 10 Web API, C# 13 |
| ORM | Entity Framework Core 10 (Code First + Migrations + Seed) |
| Banco | SQL Server 2022 |
| Autenticação | JWT Bearer + ASP.NET Identity (PasswordHasher) |
| Validação | FluentValidation |
| Mapeamento | AutoMapper |
| Documentação | Swagger / OpenAPI |
| Front-end | Razor Pages + Bootstrap 5 customizado |
| Testes | xUnit, Bogus, WebApplicationFactory |
| Controle de versão (código) | Git + GitHub (colab.: jhoseju@gmail.com) |
| Controle de versão (banco) | EF Core Migrations |

---

## 9. Identidade Visual

- **Nome**: SocialCare
- **Slogan**: *"Cuidar é transformar."*
- **Paleta sugerida**:
  - Primária: `#1F6FEB` (azul institucional)
  - Secundária: `#10B981` (verde — esperança/assistência)
  - Neutros: `#F8FAFC`, `#1E293B`
  - Alerta: `#F59E0B`
- **Tipografia**: Inter (UI) + Source Serif (títulos de relatório).
- **Logotipo**: ícone de mãos entrelaçadas formando uma casa.

---

## 10. Cronograma de Entregas

| Data | Entrega |
|---|---|
| **22/05/2026** *(hoje)* | URL do repositório Git, Diagrama de Casos de Uso |
| 29/05/2026 | Documento de escopo, Diagrama de Banco, regras de negócio |
| 12/06/2026 | Aplicação final + apresentação |

---

## 11. Diagrama de Casos de Uso — Atores e Casos

**Atores**: Administrador, Gestor, Assistente Social, Sistema Externo (ViaCEP/IBGE).

**Casos de Uso principais**:
- UC01 Autenticar-se
- UC02 Gerenciar Usuários *(Admin)*
- UC03 Cadastrar Família
- UC04 Cadastrar Membro
- UC05 Buscar Endereço por CEP *«include» ViaCEP*
- UC06 Cadastrar Programa Social *(Gestor)*
- UC07 Conceder Benefício *(Gestor)*
- UC08 Agendar Visita
- UC09 Registrar Visita
- UC10 Abrir Atendimento
- UC11 Encaminhar para Instituição Parceira
- UC12 Emitir Relatório de Famílias por Vulnerabilidade
- UC13 Emitir Relatório de Benefícios por Programa
- UC14 Emitir Relatório de Visitas por Assistente
- UC15 Auditar Operações *(Admin)*
- UC16 Consultar Indicadores Públicos *(ator externo: parceiro/BI)*
