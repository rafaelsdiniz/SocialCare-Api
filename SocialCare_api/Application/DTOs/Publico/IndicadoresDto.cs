namespace SocialCare.Application.DTOs.Publico;

public record IndicadoresDto(
    int TotalFamiliasAtivas,
    int TotalMembros,
    int TotalBeneficiosAtivos,
    int TotalProgramas,
    int TotalVisitasUltimos30Dias,
    IReadOnlyList<IndicadorPorUfDto> FamiliasPorUf,
    IReadOnlyList<IndicadorPorProgramaDto> BeneficiosPorPrograma);

public record IndicadorPorUfDto(string Uf, int Quantidade);
public record IndicadorPorProgramaDto(string Programa, int Quantidade);
