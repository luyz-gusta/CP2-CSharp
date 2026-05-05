namespace BancoDigital.API.Domain.Entities;

public class ReceberSalario : Produto
{
    public string EmpresaConveniada { get; set; } = string.Empty;
    public string CnpjEmpregador { get; set; } = string.Empty;

    public override string TipoProduto => "ReceberSalario";
}
