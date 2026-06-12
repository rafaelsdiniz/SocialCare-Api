namespace SocialCare.Application.DTOs.Publico;

public record IndicadoresDto(
    int TotalFamiliasAtivas,
    int TotalMembros,
    int TotalBeneficiosAtivos,
    int TotalProgramas,
    int TotalVisitasUltimos30Dias,
    int PopulacaoAbrangida,
    IReadOnlyList<IndicadorPorUfDto> FamiliasPorUf,
    IReadOnlyList<IndicadorPorProgramaDto> BeneficiosPorPrograma,
    IReadOnlyList<IndicadorPorMunicipioDto> FamiliasPorMunicipio,
    IReadOnlyList<IndicadorPorStatusDto> FamiliasPorStatus);

public record IndicadorPorUfDto(string Uf, int Quantidade);
public record IndicadorPorProgramaDto(string Programa, int Quantidade);
public record IndicadorPorMunicipioDto(string Municipio, int Quantidade);
public record IndicadorPorStatusDto(string Status, int Quantidade);
