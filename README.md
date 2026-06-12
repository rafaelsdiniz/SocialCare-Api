# SocialCare API

Back-end do SocialCare, um sistema de gestão de assistência social. A API cuida do
cadastro de famílias, controle de benefícios, visitas domiciliares, atendimentos,
encaminhamentos para instituições parceiras e relatórios gerenciais. Também tem
algumas rotas públicas (programas, indicadores e consulta de CEP) usadas pelo site.

Projeto feito como trabalho da disciplina de Tópicos III.

- Aluno: Rafael Silva Diniz
- Instituição: Unitins
- Disciplina: Tópicos III
- Professor: Itamar

## Tecnologias

- ASP.NET Core (.NET 10), em C#
- Entity Framework Core 10 com SQL Server
- Autenticação JWT e hashing de senha do ASP.NET Identity
- FluentValidation nas validações
- Swagger para documentar e testar os endpoints

## Pré-requisitos

- SDK do .NET 10
- SQL Server. Por padrão a connection string usa o LocalDB que acompanha o Visual
  Studio (veja `appsettings.json`). Se você usa outra instância, é só trocar a
  `DefaultConnection`.

## Como rodar

Na raiz do repositório:

```bash
# cria o banco e aplica as migrations
dotnet ef database update --project SocialCare_api

# sobe a API
dotnet run --project SocialCare_api
```

A API sobe em `http://localhost:5128` e a raiz já redireciona pro Swagger
(`http://localhost:5128/swagger`).

Se a ferramenta do EF não estiver instalada:

```bash
dotnet tool install --global dotnet-ef
```

### Primeiro acesso

No primeiro boot a API cria um usuário administrador (definido no `appsettings.json`):

- login: `admin`
- senha: `ChangeMe@123`

Também são inseridos alguns dados de exemplo pra facilitar os testes. Pra desligar,
mude `Demo:PopularNaInicializacao` para `false` no `appsettings.json`.

## Organização do código

O projeto é separado em camadas:

```
Domain/          entidades e enums do negócio
Application/     services, DTOs, validators e interfaces (casos de uso)
Infrastructure/  EF Core, repositórios, integrações externas, JWT e identidade
Controllers/     endpoints REST (a pasta Publico/ tem as rotas abertas)
Middlewares/     tratamento de exceções e log de auditoria
Extensions/      configuração de DI, Swagger, JWT e CORS
```

## Perfis de acesso

- Administrador: usuários, perfis e auditoria
- Gestor: benefícios, programas, instituições parceiras e relatórios
- Assistente Social: famílias, membros, visitas, atendimentos e encaminhamentos

## Integrações externas

- ViaCEP: preenche o endereço a partir do CEP
- IBGE: estados e municípios
- BrasilAPI: consulta de CNPJ das instituições parceiras
- Nominatim (OpenStreetMap): coordenadas usadas no mapa
- Portal da Transparência: consulta opcional (precisa de chave de API)

## Front-end

A interface fica em outro repositório (`socialcare-web`), feito em Next.js. O CORS
já vem liberado para `http://localhost:3000`.
