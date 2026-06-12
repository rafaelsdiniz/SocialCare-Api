namespace SocialCare.Application.Common;

/// <summary>Validação e formatação de CPF e CNPJ (algoritmo dos dígitos verificadores, sem API externa).</summary>
public static class DocumentoFiscal
{
    public static string SomenteDigitos(string? valor)
        => string.IsNullOrEmpty(valor) ? string.Empty : new string(valor.Where(char.IsDigit).ToArray());

    public static bool CpfValido(string? cpf)
    {
        var d = SomenteDigitos(cpf);
        if (d.Length != 11 || d.Distinct().Count() == 1) return false;

        var numeros = d.Select(c => c - '0').ToArray();
        return numeros[9] == DigitoVerificador(numeros, 9, 10)
            && numeros[10] == DigitoVerificador(numeros, 10, 11);
    }

    public static bool CnpjValido(string? cnpj)
    {
        var d = SomenteDigitos(cnpj);
        if (d.Length != 14 || d.Distinct().Count() == 1) return false;

        var numeros = d.Select(c => c - '0').ToArray();
        int[] pesos1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] pesos2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        return numeros[12] == DigitoPorPeso(numeros, pesos1)
            && numeros[13] == DigitoPorPeso(numeros, pesos2);
    }

    public static string FormatarCnpj(string? cnpj)
    {
        var d = SomenteDigitos(cnpj);
        return d.Length != 14
            ? d
            : $"{d[..2]}.{d[2..5]}.{d[5..8]}/{d[8..12]}-{d[12..]}";
    }

    public static string FormatarCpf(string? cpf)
    {
        var d = SomenteDigitos(cpf);
        return d.Length != 11
            ? d
            : $"{d[..3]}.{d[3..6]}.{d[6..9]}-{d[9..]}";
    }

    private static int DigitoVerificador(int[] numeros, int ate, int pesoInicial)
    {
        var soma = 0;
        for (var i = 0; i < ate; i++)
            soma += numeros[i] * (pesoInicial - i);

        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }

    private static int DigitoPorPeso(int[] numeros, int[] pesos)
    {
        var soma = 0;
        for (var i = 0; i < pesos.Length; i++)
            soma += numeros[i] * pesos[i];

        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }
}
