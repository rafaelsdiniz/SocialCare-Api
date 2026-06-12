



Universidade Estadual do Tocantins
Tópicos III







SocialCare 
Sistema de Gestão de Assistência Social

Rafael Silva Diniz











Palmas – TO
Maio/2026
Sumário
Nenhuma entrada de sumário foi encontrada.



























1. Identificação do Projeto
CampoDescriçãoNome do ProjetoSocialCare — Sistema de Gestão de Assistência SocialSlogan“Cuidar é transformar.”Tipo de SistemaAplicação Web + API RESTRepositórioGitHub — rafaelsdiniz/socialcare-apiTecnologias PrincipaisASP.NET Core 10, C#, EF Core, SQL Server, JWT, Razor Pages e Bootstrap 5
2. Objetivo Geral
Desenvolver uma plataforma web para gerenciamento de assistência social em órgãos públicos e organizações sociais, centralizando o cadastro de famílias, benefícios, atendimentos, visitas domiciliares e relatórios gerenciais, promovendo maior controle, rastreabilidade e eficiência operacional.

3. Objetivos Específicos
• Cadastrar famílias, membros e composição de renda;
• Registrar situações de vulnerabilidade social;
• Gerenciar programas sociais e concessão de benefícios;
• Registrar atendimentos e visitas domiciliares;
• Realizar encaminhamentos para instituições parceiras;
• Emitir relatórios gerenciais e indicadores;
• Disponibilizar endpoints públicos com dados anonimizados;
• Garantir controle de acesso por perfil de usuário;
• Registrar auditoria das operações sensíveis;
• Integrar APIs externas para validação e preenchimento automático de dados.

4. Justificativa
Muitos órgãos públicos e organizações sociais ainda realizam o controle de atendimentos utilizando planilhas e documentos físicos, o que gera problemas como perda de histórico, duplicidade de informações, dificuldade na emissão de relatórios e baixa confiabilidade dos dados.
O sistema SocialCare busca solucionar esses problemas por meio de uma plataforma centralizada, permitindo maior organização, controle operacional, rastreabilidade e apoio à tomada de decisão.
Além disso, a aplicação contribuirá para maior transparência administrativa através da disponibilização de indicadores anonimizados.

5. Público-Alvo
PúblicoDescriçãoAdministradorGerencia usuários, permissões e auditoriaGestorGerencia programas sociais e benefíciosAssistente SocialRealiza cadastros, visitas e atendimentosInstituições ParceirasRecebem encaminhamentosÓrgãos PúblicosUtilizam indicadores e relatóriosAvaliador AcadêmicoResponsável pela avaliação do projeto
6. Funcionalidades do Sistema
6.1 Autenticação e Controle de Acesso
• Login com e-mail e senha;
• Autenticação via JWT;
• Controle de permissões por perfil;
• Expiração de sessão.
6.2 Cadastro Familiar
• Cadastro de famílias;
• Cadastro de membros familiares;
• Registro de documentos;
• Controle de renda familiar;
• Cálculo automático de renda per capita;
• Registro de vulnerabilidades sociais.
6.3 Programas e Benefícios
• Cadastro de programas sociais;
• Concessão de benefícios;
• Controle de status dos benefícios;
• Cadastro de instituições parceiras.
6.4 Operação Social
• Agendamento de visitas domiciliares;
• Registro de atendimentos;
• Registro de visitas realizadas;
• Encaminhamento para instituições parceiras.
6.5 Relatórios e Indicadores
• Relatórios de vulnerabilidade;
• Relatórios de benefícios;
• Relatórios de produtividade;
• Endpoints públicos de indicadores.
6.6 Administração
• Gerenciamento de usuários;
• Controle de perfis;
• Consulta de auditoria;
• Seed inicial do sistema.

7. Fora do Escopo
Os itens abaixo não serão implementados nesta versão do projeto:
• Aplicativo mobile;
• Integração financeira para pagamento de benefícios;
• Sistema de notificações por SMS ou e-mail;
• Inteligência artificial;
• Multi-tenant;
• Chat interno;
• Geolocalização;
• Assinatura digital;
• Conformidade LGPD completa;
• Importação em massa por planilhas.

8. Requisitos Funcionais

9. Requisitos Não Funcionais
IDCategoriaDescriçãoRNF01SegurançaSenhas armazenadas com hashRNF02SegurançaAutenticação JWTRNF03SegurançaControle de acesso por perfilRNF04SegurançaDados públicos anonimizadosRNF05PerformancePaginação nas listagensRNF06PerformanceTempo médio de resposta inferior a 1 segundoRNF07UsabilidadeInterface responsivaRNF08UsabilidadeMensagens de erro padronizadasRNF09CompatibilidadeCompatível com navegadores modernosRNF10ManutenibilidadeArquitetura em camadasRNF11ManutenibilidadeUso de migrationsRNF12DocumentaçãoSwagger/OpenAPIRNF13TestabilidadeTestes unitáriosRNF14ConfiabilidadeTratamento global de exceçõesRNF15ConfiabilidadeValidação de dadosRNF16PortabilidadeExecução em Windows e Linux
10. Regras de Negócio
IDRegraRN01Toda família deve possuir um responsávelRN02O CPF deve ser único no sistemaRN03Apenas Gestor ou Administrador podem conceder benefíciosRN04Benefícios devem possuir data de inícioRN05A renda per capita será calculada automaticamenteRN06Uma família pode possuir múltiplas vulnerabilidadesRN07Senhas devem possuir no mínimo 8 caracteresRN08Usuários podem possuir múltiplos perfisRN09Visitas só podem ser concluídas após agendamentoRN10Encaminhamentos devem utilizar instituições ativasRN11O sistema utilizará soft deleteRN12Operações sensíveis devem gerar auditoriaRN13Endpoints públicos não devem retornar dados pessoais
11. Premissas
• O ambiente possuirá SQL Server instalado;
• APIs externas estarão disponíveis;
• Usuários possuirão acesso à internet;
• O sistema utilizará idioma português brasileiro;
• O ambiente será single-tenant;
• Serão aplicadas boas práticas básicas de segurança.

12. Restrições
TipoRestriçãoTecnológicaUso obrigatório de ASP.NET Core e EF CoreTecnológicaFront-end em Razor PagesAcadêmicaMínimo de 18 entidadesAcadêmicaMínimo de 3 endpoints públicosAcadêmicaIntegração com pelo menos 1 API externaTemporalEntrega final em 12/06/2026
13. Arquitetura do Sistema
O sistema será desenvolvido utilizando arquitetura em camadas baseada nos princípios da Clean Architecture, promovendo separação de responsabilidades, organização do código e facilidade de manutenção.
Estrutura da Solução
• SocialCare.API — API REST;
• SocialCare.Domain — entidades e regras de negócio;
• SocialCare.Application — serviços, DTOs e validações;
• SocialCare.Infrastructure — persistência e integrações;
• SocialCare.Web — interface web;
• SocialCare.Tests — testes automatizados.

14. Tecnologias Utilizadas
CamadaTecnologiaBack-endASP.NET Core 10LinguagemC#ORMEntity Framework CoreBanco de DadosSQL Server 2022AutenticaçãoJWTFront-endRazor PagesEstilizaçãoBootstrap 5TestesxUnitVersionamentoGit e GitHub
15. Cronograma
DataEntregávelStatus22/05/2026Repositório e Caso de UsoEntregue29/05/2026Documento de Escopo e DEREntregue12/06/2026Sistema FinalPendente
16. Critérios de Aceitação
Para aprovação do projeto, deverão ser entregues:
• Repositório Git público;
• Documento de escopo;
• Diagrama de banco de dados;
• Mínimo de 18 entidades;
• Endpoints públicos e autenticados;
• Integração com API externa;
• Autenticação JWT funcional;
• Interface responsiva;
• Swagger configurado;
• README com instruções de execução.

17. Riscos do Projeto
IDRiscoMitigaçãoR01Indisponibilidade de APIs externasFallback manualR02Atraso no front-endPriorização das telas principaisR03Problemas com migrationsRevisão antes de mergeR04Dificuldade com Razor PagesUso de scaffoldingR05Crescimento excessivo do escopoControle de mudanças
18. Glossário
TermoDefiniçãoCRASCentro de Referência de Assistência SocialCREASCentro de Referência Especializado de Assistência SocialCadÚnicoCadastro Único para Programas SociaisNISNúmero de Identificação SocialJWTJSON Web TokenDERDiagrama Entidade-RelacionamentoSeedCarga inicial de dados
19. Controle de Versão
VersãoDataAutorAlteração1.029/05/2026Rafael DinizVersão inicial
