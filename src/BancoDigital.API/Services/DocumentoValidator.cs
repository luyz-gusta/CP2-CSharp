using System.Text.RegularExpressions;

namespace BancoDigital.API.Services;

public static class DocumentoValidator
{
    public static string LimparCpf(string cpf) => Regex.Replace(cpf ?? string.Empty, @"\D", "");
    public static string LimparCnpj(string cnpj) => Regex.Replace(cnpj ?? string.Empty, @"\D", "");

    public static bool CpfValido(string cpf)
    {
        cpf = LimparCpf(cpf);
        if (cpf.Length != 11 || cpf.Distinct().Count() == 1) return false;

        int[] mult1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] mult2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        var tempCpf = cpf.Substring(0, 9);
        var soma = 0;
        for (int i = 0; i < 9; i++) soma += int.Parse(tempCpf[i].ToString()) * mult1[i];
        var resto = soma % 11;
        var digito1 = resto < 2 ? 0 : 11 - resto;

        tempCpf += digito1;
        soma = 0;
        for (int i = 0; i < 10; i++) soma += int.Parse(tempCpf[i].ToString()) * mult2[i];
        resto = soma % 11;
        var digito2 = resto < 2 ? 0 : 11 - resto;

        return cpf.EndsWith($"{digito1}{digito2}");
    }

    public static bool CnpjValido(string cnpj)
    {
        cnpj = LimparCnpj(cnpj);
        if (cnpj.Length != 14 || cnpj.Distinct().Count() == 1) return false;

        int[] mult1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] mult2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        var tempCnpj = cnpj.Substring(0, 12);
        var soma = 0;
        for (int i = 0; i < 12; i++) soma += int.Parse(tempCnpj[i].ToString()) * mult1[i];
        var resto = soma % 11;
        var digito1 = resto < 2 ? 0 : 11 - resto;

        tempCnpj += digito1;
        soma = 0;
        for (int i = 0; i < 13; i++) soma += int.Parse(tempCnpj[i].ToString()) * mult2[i];
        resto = soma % 11;
        var digito2 = resto < 2 ? 0 : 11 - resto;

        return cnpj.EndsWith($"{digito1}{digito2}");
    }
}
