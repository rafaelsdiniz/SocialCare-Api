using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SocialCare.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "estado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoIbge = table.Column<int>(type: "int", nullable: false),
                    Sigla = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Regiao = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estado", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "instituicao_parceira",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Cnpj = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: false),
                    AreaAtuacao = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ResponsavelContato = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    EnderecoCompleto = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_instituicao_parceira", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "parentesco",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parentesco", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "perfil",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_perfil", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "programa_social",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Requisitos = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OrgaoResponsavel = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ValorPadrao = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    DuracaoMesesPadrao = table.Column<int>(type: "int", nullable: true),
                    VigenciaInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VigenciaFim = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_programa_social", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tipo_documento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Sigla = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MascaraValidacao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipo_documento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tipo_renda",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ConsideradaParaCalculo = table.Column<bool>(type: "bit", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipo_renda", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Login = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SenhaHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UltimoLoginEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "vulnerabilidade",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Severidade = table.Column<int>(type: "int", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vulnerabilidade", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "municipio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoIbge = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    EstadoId = table.Column<int>(type: "int", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_municipio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_municipio_estado_EstadoId",
                        column: x => x.EstadoId,
                        principalTable: "estado",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "log_auditoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Entidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntidadeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DadosAntes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DadosDepois = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnderecoIp = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_log_auditoria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_log_auditoria_usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "usuario_perfil",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    PerfilId = table.Column<int>(type: "int", nullable: false),
                    AtribuidoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario_perfil", x => new { x.UsuarioId, x.PerfilId });
                    table.ForeignKey(
                        name: "FK_usuario_perfil_perfil_PerfilId",
                        column: x => x.PerfilId,
                        principalTable: "perfil",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_usuario_perfil_usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "atendimento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataAtendimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Parecer = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Demanda = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Remoto = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FamiliaId = table.Column<int>(type: "int", nullable: false),
                    AssistenteResponsavelId = table.Column<int>(type: "int", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_atendimento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_atendimento_usuario_AssistenteResponsavelId",
                        column: x => x.AssistenteResponsavelId,
                        principalTable: "usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "beneficio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Valor = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MotivoEncerramento = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FamiliaId = table.Column<int>(type: "int", nullable: false),
                    ProgramaSocialId = table.Column<int>(type: "int", nullable: false),
                    AprovadoPorUsuarioId = table.Column<int>(type: "int", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_beneficio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_beneficio_programa_social_ProgramaSocialId",
                        column: x => x.ProgramaSocialId,
                        principalTable: "programa_social",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_beneficio_usuario_AprovadoPorUsuarioId",
                        column: x => x.AprovadoPorUsuarioId,
                        principalTable: "usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "documento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numero = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrgaoEmissor = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    DataEmissao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataValidade = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MembroId = table.Column<int>(type: "int", nullable: false),
                    TipoDocumentoId = table.Column<int>(type: "int", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_documento_tipo_documento_TipoDocumentoId",
                        column: x => x.TipoDocumentoId,
                        principalTable: "tipo_documento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "encaminhamento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataEncaminhamento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Demanda = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DataRetorno = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Retorno = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FamiliaId = table.Column<int>(type: "int", nullable: false),
                    InstituicaoParceiraId = table.Column<int>(type: "int", nullable: false),
                    MembroId = table.Column<int>(type: "int", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_encaminhamento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_encaminhamento_instituicao_parceira_InstituicaoParceiraId",
                        column: x => x.InstituicaoParceiraId,
                        principalTable: "instituicao_parceira",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "endereco",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cep = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    Logradouro = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Complemento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Bairro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PontoReferencia = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MunicipioId = table.Column<int>(type: "int", nullable: false),
                    FamiliaId = table.Column<int>(type: "int", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_endereco", x => x.Id);
                    table.ForeignKey(
                        name: "FK_endereco_municipio_MunicipioId",
                        column: x => x.MunicipioId,
                        principalTable: "municipio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "familia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoFamiliar = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NomeResponsavel = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    QuantidadeMembros = table.Column<int>(type: "int", nullable: false),
                    RendaTotalMensal = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    RendaPerCapita = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MembroResponsavelId = table.Column<int>(type: "int", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_familia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "familia_vulnerabilidade",
                columns: table => new
                {
                    FamiliaId = table.Column<int>(type: "int", nullable: false),
                    VulnerabilidadeId = table.Column<int>(type: "int", nullable: false),
                    IdentificadaEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Resolvida = table.Column<bool>(type: "bit", nullable: false),
                    ResolvidaEm = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_familia_vulnerabilidade", x => new { x.FamiliaId, x.VulnerabilidadeId });
                    table.ForeignKey(
                        name: "FK_familia_vulnerabilidade_familia_FamiliaId",
                        column: x => x.FamiliaId,
                        principalTable: "familia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_familia_vulnerabilidade_vulnerabilidade_VulnerabilidadeId",
                        column: x => x.VulnerabilidadeId,
                        principalTable: "vulnerabilidade",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "membro",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sexo = table.Column<int>(type: "int", nullable: false),
                    EstadoCivil = table.Column<int>(type: "int", nullable: false),
                    NomeMae = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    NomePai = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Escolaridade = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Ocupacao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PessoaComDeficiencia = table.Column<bool>(type: "bit", nullable: false),
                    DescricaoDeficiencia = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FamiliaId = table.Column<int>(type: "int", nullable: false),
                    ParentescoId = table.Column<int>(type: "int", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_membro", x => x.Id);
                    table.ForeignKey(
                        name: "FK_membro_familia_FamiliaId",
                        column: x => x.FamiliaId,
                        principalTable: "familia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_membro_parentesco_ParentescoId",
                        column: x => x.ParentescoId,
                        principalTable: "parentesco",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "visita",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataAgendada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataRealizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Observacoes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Encaminhamentos = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FamiliaId = table.Column<int>(type: "int", nullable: false),
                    AssistenteResponsavelId = table.Column<int>(type: "int", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_visita", x => x.Id);
                    table.ForeignKey(
                        name: "FK_visita_familia_FamiliaId",
                        column: x => x.FamiliaId,
                        principalTable: "familia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_visita_usuario_AssistenteResponsavelId",
                        column: x => x.AssistenteResponsavelId,
                        principalTable: "usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "renda",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Valor = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    MesReferencia = table.Column<int>(type: "int", nullable: false),
                    AnoReferencia = table.Column<int>(type: "int", nullable: false),
                    Fonte = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Observacao = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MembroId = table.Column<int>(type: "int", nullable: false),
                    TipoRendaId = table.Column<int>(type: "int", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_renda", x => x.Id);
                    table.ForeignKey(
                        name: "FK_renda_membro_MembroId",
                        column: x => x.MembroId,
                        principalTable: "membro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_renda_tipo_renda_TipoRendaId",
                        column: x => x.TipoRendaId,
                        principalTable: "tipo_renda",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "parentesco",
                columns: new[] { "Id", "Ativo", "AtualizadoEm", "CriadoEm", "Descricao", "Nome" },
                values: new object[,]
                {
                    { 1, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Responsável familiar", "Responsável" },
                    { 2, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cônjuge" },
                    { 3, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Filho(a)" },
                    { 4, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Enteado(a)" },
                    { 5, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Pai/Mãe" },
                    { 6, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Avô/Avó" },
                    { 7, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Neto(a)" },
                    { 8, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Irmão/Irmã" },
                    { 9, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Outro" }
                });

            migrationBuilder.InsertData(
                table: "perfil",
                columns: new[] { "Id", "Ativo", "AtualizadoEm", "CriadoEm", "Descricao", "Nome" },
                values: new object[,]
                {
                    { 1, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Acesso total ao sistema", "Administrador" },
                    { 2, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Programas, benefícios e relatórios", "Gestor" },
                    { 3, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Atuação direta com famílias e visitas", "AssistenteSocial" }
                });

            migrationBuilder.InsertData(
                table: "programa_social",
                columns: new[] { "Id", "Ativo", "AtualizadoEm", "CriadoEm", "Descricao", "DuracaoMesesPadrao", "Nome", "OrgaoResponsavel", "Requisitos", "ValorPadrao", "VigenciaFim", "VigenciaInicio" },
                values: new object[,]
                {
                    { 1, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Transferência de renda para famílias em vulnerabilidade", null, "Bolsa Família", "MDS", null, null, null, null },
                    { 2, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Auxílio para compra de gás de cozinha", null, "Auxílio Gás", "MDS", null, null, null, null },
                    { 3, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "BPC - Benefício de Prestação Continuada", "INSS", null, null, null, null },
                    { 4, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Programa Leite das Crianças", "Secretaria Estadual", null, null, null, null },
                    { 5, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Tarifa Social de Energia", "ANEEL", null, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "tipo_documento",
                columns: new[] { "Id", "Ativo", "AtualizadoEm", "CriadoEm", "MascaraValidacao", "Nome", "Sigla" },
                values: new object[,]
                {
                    { 1, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "###.###.###-##", "CPF", "CPF" },
                    { 2, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "RG", "RG" },
                    { 3, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Número de Identificação Social", "NIS" },
                    { 4, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Certidão de Nascimento", "CertidaoNasc" },
                    { 5, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Carteira de Trabalho", "CTPS" },
                    { 6, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Título de Eleitor", "TituloEleitor" }
                });

            migrationBuilder.InsertData(
                table: "tipo_renda",
                columns: new[] { "Id", "Ativo", "AtualizadoEm", "ConsideradaParaCalculo", "CriadoEm", "Descricao", "Nome" },
                values: new object[,]
                {
                    { 1, true, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Salário CLT" },
                    { 2, true, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Autônomo" },
                    { 3, true, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Aposentadoria" },
                    { 4, true, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Pensão" },
                    { 5, true, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Bolsa Família" },
                    { 6, true, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Benefício de Prestação Continuada" },
                    { 7, true, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Trabalho Informal" }
                });

            migrationBuilder.InsertData(
                table: "vulnerabilidade",
                columns: new[] { "Id", "Ativo", "AtualizadoEm", "CriadoEm", "Descricao", "Nome", "Severidade" },
                values: new object[,]
                {
                    { 1, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Insegurança alimentar", 3 },
                    { 2, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Moradia precária", 3 },
                    { 3, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Violência doméstica", 4 },
                    { 4, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Trabalho infantil", 4 },
                    { 5, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Situação de rua", 4 },
                    { 6, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Desemprego prolongado", 2 },
                    { 7, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Evasão escolar", 3 },
                    { 8, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Dependência química", 4 },
                    { 9, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Idoso em situação de risco", 4 },
                    { 10, true, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Gestante sem acompanhamento", 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_atendimento_AssistenteResponsavelId",
                table: "atendimento",
                column: "AssistenteResponsavelId");

            migrationBuilder.CreateIndex(
                name: "IX_atendimento_DataAtendimento",
                table: "atendimento",
                column: "DataAtendimento");

            migrationBuilder.CreateIndex(
                name: "IX_atendimento_FamiliaId",
                table: "atendimento",
                column: "FamiliaId");

            migrationBuilder.CreateIndex(
                name: "IX_beneficio_AprovadoPorUsuarioId",
                table: "beneficio",
                column: "AprovadoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_beneficio_FamiliaId_ProgramaSocialId_DataInicio",
                table: "beneficio",
                columns: new[] { "FamiliaId", "ProgramaSocialId", "DataInicio" });

            migrationBuilder.CreateIndex(
                name: "IX_beneficio_ProgramaSocialId",
                table: "beneficio",
                column: "ProgramaSocialId");

            migrationBuilder.CreateIndex(
                name: "IX_documento_MembroId",
                table: "documento",
                column: "MembroId");

            migrationBuilder.CreateIndex(
                name: "IX_documento_TipoDocumentoId_Numero",
                table: "documento",
                columns: new[] { "TipoDocumentoId", "Numero" });

            migrationBuilder.CreateIndex(
                name: "IX_encaminhamento_DataEncaminhamento",
                table: "encaminhamento",
                column: "DataEncaminhamento");

            migrationBuilder.CreateIndex(
                name: "IX_encaminhamento_FamiliaId",
                table: "encaminhamento",
                column: "FamiliaId");

            migrationBuilder.CreateIndex(
                name: "IX_encaminhamento_InstituicaoParceiraId",
                table: "encaminhamento",
                column: "InstituicaoParceiraId");

            migrationBuilder.CreateIndex(
                name: "IX_encaminhamento_MembroId",
                table: "encaminhamento",
                column: "MembroId");

            migrationBuilder.CreateIndex(
                name: "IX_endereco_Cep",
                table: "endereco",
                column: "Cep");

            migrationBuilder.CreateIndex(
                name: "IX_endereco_FamiliaId",
                table: "endereco",
                column: "FamiliaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_endereco_MunicipioId",
                table: "endereco",
                column: "MunicipioId");

            migrationBuilder.CreateIndex(
                name: "IX_estado_CodigoIbge",
                table: "estado",
                column: "CodigoIbge",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_estado_Sigla",
                table: "estado",
                column: "Sigla",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_familia_CodigoFamiliar",
                table: "familia",
                column: "CodigoFamiliar",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_familia_MembroResponsavelId",
                table: "familia",
                column: "MembroResponsavelId");

            migrationBuilder.CreateIndex(
                name: "IX_familia_vulnerabilidade_VulnerabilidadeId",
                table: "familia_vulnerabilidade",
                column: "VulnerabilidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_instituicao_parceira_Cnpj",
                table: "instituicao_parceira",
                column: "Cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_log_auditoria_CriadoEm",
                table: "log_auditoria",
                column: "CriadoEm");

            migrationBuilder.CreateIndex(
                name: "IX_log_auditoria_Entidade_EntidadeId",
                table: "log_auditoria",
                columns: new[] { "Entidade", "EntidadeId" });

            migrationBuilder.CreateIndex(
                name: "IX_log_auditoria_UsuarioId",
                table: "log_auditoria",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_membro_FamiliaId",
                table: "membro",
                column: "FamiliaId");

            migrationBuilder.CreateIndex(
                name: "IX_membro_ParentescoId",
                table: "membro",
                column: "ParentescoId");

            migrationBuilder.CreateIndex(
                name: "IX_municipio_CodigoIbge",
                table: "municipio",
                column: "CodigoIbge",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_municipio_EstadoId_Nome",
                table: "municipio",
                columns: new[] { "EstadoId", "Nome" });

            migrationBuilder.CreateIndex(
                name: "IX_parentesco_Nome",
                table: "parentesco",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_perfil_Nome",
                table: "perfil",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_programa_social_Nome",
                table: "programa_social",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_renda_MembroId_AnoReferencia_MesReferencia",
                table: "renda",
                columns: new[] { "MembroId", "AnoReferencia", "MesReferencia" });

            migrationBuilder.CreateIndex(
                name: "IX_renda_TipoRendaId",
                table: "renda",
                column: "TipoRendaId");

            migrationBuilder.CreateIndex(
                name: "IX_tipo_documento_Sigla",
                table: "tipo_documento",
                column: "Sigla",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tipo_renda_Nome",
                table: "tipo_renda",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuario_Email",
                table: "usuario",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuario_Login",
                table: "usuario",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuario_perfil_PerfilId",
                table: "usuario_perfil",
                column: "PerfilId");

            migrationBuilder.CreateIndex(
                name: "IX_visita_AssistenteResponsavelId",
                table: "visita",
                column: "AssistenteResponsavelId");

            migrationBuilder.CreateIndex(
                name: "IX_visita_DataAgendada",
                table: "visita",
                column: "DataAgendada");

            migrationBuilder.CreateIndex(
                name: "IX_visita_FamiliaId",
                table: "visita",
                column: "FamiliaId");

            migrationBuilder.CreateIndex(
                name: "IX_vulnerabilidade_Nome",
                table: "vulnerabilidade",
                column: "Nome",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_atendimento_familia_FamiliaId",
                table: "atendimento",
                column: "FamiliaId",
                principalTable: "familia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_beneficio_familia_FamiliaId",
                table: "beneficio",
                column: "FamiliaId",
                principalTable: "familia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_documento_membro_MembroId",
                table: "documento",
                column: "MembroId",
                principalTable: "membro",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_encaminhamento_familia_FamiliaId",
                table: "encaminhamento",
                column: "FamiliaId",
                principalTable: "familia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_encaminhamento_membro_MembroId",
                table: "encaminhamento",
                column: "MembroId",
                principalTable: "membro",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_endereco_familia_FamiliaId",
                table: "endereco",
                column: "FamiliaId",
                principalTable: "familia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_familia_membro_MembroResponsavelId",
                table: "familia",
                column: "MembroResponsavelId",
                principalTable: "membro",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_membro_familia_FamiliaId",
                table: "membro");

            migrationBuilder.DropTable(
                name: "atendimento");

            migrationBuilder.DropTable(
                name: "beneficio");

            migrationBuilder.DropTable(
                name: "documento");

            migrationBuilder.DropTable(
                name: "encaminhamento");

            migrationBuilder.DropTable(
                name: "endereco");

            migrationBuilder.DropTable(
                name: "familia_vulnerabilidade");

            migrationBuilder.DropTable(
                name: "log_auditoria");

            migrationBuilder.DropTable(
                name: "renda");

            migrationBuilder.DropTable(
                name: "usuario_perfil");

            migrationBuilder.DropTable(
                name: "visita");

            migrationBuilder.DropTable(
                name: "programa_social");

            migrationBuilder.DropTable(
                name: "tipo_documento");

            migrationBuilder.DropTable(
                name: "instituicao_parceira");

            migrationBuilder.DropTable(
                name: "municipio");

            migrationBuilder.DropTable(
                name: "vulnerabilidade");

            migrationBuilder.DropTable(
                name: "tipo_renda");

            migrationBuilder.DropTable(
                name: "perfil");

            migrationBuilder.DropTable(
                name: "usuario");

            migrationBuilder.DropTable(
                name: "estado");

            migrationBuilder.DropTable(
                name: "familia");

            migrationBuilder.DropTable(
                name: "membro");

            migrationBuilder.DropTable(
                name: "parentesco");
        }
    }
}
