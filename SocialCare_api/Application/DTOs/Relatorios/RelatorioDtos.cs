namespace SocialCare.Application.DTOs.Relatorios;

public record FamiliasPorVulnerabilidadeDto(
    int VulnerabilidadeId,
    string Vulnerabilidade,
    int Severidade,
    int QuantidadeFamilias);

public record BeneficiosPorProgramaDto(
    int ProgramaSocialId,
    string Programa,
    int QuantidadeTotal,
    int QuantidadeAtivos,
    decimal ValorTotalAtivos);

public record VisitasPorAssistenteDto(
    int AssistenteId,
    string Assistente,
    int Total,
    int Realizadas,
    int Agendadas,
    int Canceladas);
