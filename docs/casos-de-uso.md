# SocialCare — Diagrama de Casos de Uso

> Fonte canônica: [`casos-de-uso.puml`](./casos-de-uso.puml) (PlantUML).
> Esta versão Mermaid renderiza diretamente no GitHub.

## Atores

| Ator | Tipo | Descrição |
|---|---|---|
| **Administrador** | primário | Gerencia usuários, audita logs. |
| **Gestor** | primário | Programas, benefícios e relatórios. |
| **Assistente Social** | primário | Famílias, membros, visitas, atendimentos, encaminhamentos. |
| **Parceiro / BI** | externo | Consome indicadores públicos via API. |
| **ViaCEP** | sistema externo | Autopreenchimento de endereço por CEP. |
| **IBGE** | sistema externo | Catálogo de UFs e Municípios. |

## Casos de uso

| ID | Caso de Uso | Atores |
|---|---|---|
| UC01 | Autenticar-se | Admin, Gestor, Assist. Social |
| UC02 | Gerenciar Usuários | Admin |
| UC03 | Cadastrar Família | Assist. Social |
| UC04 | Cadastrar Membro | Assist. Social |
| UC05 | Buscar Endereço por CEP *(«include» ViaCEP)* | Assist. Social |
| UC06 | Cadastrar Programa Social | Gestor |
| UC07 | Conceder Benefício | Gestor |
| UC08 | Agendar Visita | Assist. Social |
| UC09 | Registrar Visita *(«extend» UC08)* | Assist. Social |
| UC10 | Abrir Atendimento | Assist. Social |
| UC11 | Encaminhar para Instituição Parceira | Assist. Social |
| UC12 | Relatório: Famílias por Vulnerabilidade | Gestor |
| UC13 | Relatório: Benefícios por Programa | Gestor |
| UC14 | Relatório: Visitas por Assistente | Gestor |
| UC15 | Auditar Operações | Admin |
| UC16 | Consultar Indicadores Públicos | Parceiro/BI |
| UC17 | Carregar UF/Município *(«include» IBGE)* | Sistema |

## Diagrama (Mermaid)

```mermaid
graph LR
    %% ===== ATORES =====
    ADM(["👤 Administrador"]):::actor
    GES(["👤 Gestor"]):::actor
    ASS(["👤 Assistente Social"]):::actor
    PARC(["👤 Parceiro / BI"]):::actor
    VIACEP(["🌐 ViaCEP"]):::ext
    IBGE(["🌐 IBGE"]):::ext

    %% ===== CASOS DE USO =====
    subgraph SC["Sistema SocialCare"]
      direction TB

      subgraph AUTH["Autenticação & Acesso"]
        UC01(["UC01<br/>Autenticar-se"]):::uc
        UC02(["UC02<br/>Gerenciar Usuários"]):::uc
        UC15(["UC15<br/>Auditar Operações"]):::uc
      end

      subgraph FAM["Cadastro Familiar"]
        UC03(["UC03<br/>Cadastrar Família"]):::uc
        UC04(["UC04<br/>Cadastrar Membro"]):::uc
        UC05(["UC05<br/>Buscar Endereço por CEP"]):::uc
        UC17(["UC17<br/>Carregar UF/Município"]):::uc
      end

      subgraph PROG["Programas & Benefícios"]
        UC06(["UC06<br/>Cadastrar Programa Social"]):::uc
        UC07(["UC07<br/>Conceder Benefício"]):::uc
      end

      subgraph ATEND["Atendimento Social"]
        UC08(["UC08<br/>Agendar Visita"]):::uc
        UC09(["UC09<br/>Registrar Visita"]):::uc
        UC10(["UC10<br/>Abrir Atendimento"]):::uc
        UC11(["UC11<br/>Encaminhar p/ Instituição"]):::uc
      end

      subgraph REL["Relatórios & Indicadores"]
        UC12(["UC12<br/>Relat. Vulnerabilidade"]):::uc
        UC13(["UC13<br/>Relat. Benefícios"]):::uc
        UC14(["UC14<br/>Relat. Visitas"]):::uc
        UC16(["UC16<br/>Indicadores Públicos"]):::uc
      end
    end

    %% ===== ASSOCIAÇÕES =====
    ADM --- UC01
    ADM --- UC02
    ADM --- UC15

    GES --- UC01
    GES --- UC06
    GES --- UC07
    GES --- UC12
    GES --- UC13
    GES --- UC14

    ASS --- UC01
    ASS --- UC03
    ASS --- UC04
    ASS --- UC08
    ASS --- UC09
    ASS --- UC10
    ASS --- UC11

    PARC --- UC16

    %% ===== INCLUDE / EXTEND / EXTERNOS =====
    UC05 -. include .-> VIACEP
    UC17 -. include .-> IBGE
    UC03 -. include .-> UC05
    UC03 -. include .-> UC17
    UC09 -. extend  .-> UC08

    classDef actor  fill:#F8FAFC,stroke:#1E293B,stroke-width:2px,color:#1E293B;
    classDef ext    fill:#FEF3C7,stroke:#92400E,stroke-width:2px,color:#92400E;
    classDef uc     fill:#EFF6FF,stroke:#1F6FEB,stroke-width:1.5px,color:#1E293B;
```

## Como exportar como imagem

### Opção A — PlantUML online (recomendado para o PDF da entrega)

1. Abra <https://www.plantuml.com/plantuml/uml/>
2. Cole o conteúdo de [`casos-de-uso.puml`](./casos-de-uso.puml).
3. Baixe como **PNG** ou **SVG**.

### Opção B — VS Code

Instale a extensão **PlantUML** (`jebbs.plantuml`), abra o `.puml` e use
`Alt+D` para preview / `Ctrl+Shift+P → PlantUML: Export Current Diagram`.

### Opção C — Linha de comando

```powershell
java -jar plantuml.jar docs\casos-de-uso.puml -tpng
```
