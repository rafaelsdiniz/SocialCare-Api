namespace SocialCare.Application.Common;

/// <summary>Resultado paginado padrão para endpoints de listagem.</summary>
public record PagedResult<T>(
    IReadOnlyList<T> Itens,
    int Pagina,
    int TamanhoPagina,
    int TotalItens)
{
    public int TotalPaginas => TamanhoPagina <= 0 ? 0 : (int)Math.Ceiling(TotalItens / (double)TamanhoPagina);
}

/// <summary>Parâmetros de paginação recebidos via query string.</summary>
public record PaginacaoQuery
{
    private const int TamanhoMaximo = 100;
    private int _tamanhoPagina = 20;

    public int Pagina { get; init; } = 1;

    public int TamanhoPagina
    {
        get => _tamanhoPagina;
        init => _tamanhoPagina = value is < 1 or > TamanhoMaximo ? 20 : value;
    }

    public int Skip => (Math.Max(Pagina, 1) - 1) * TamanhoPagina;
}
