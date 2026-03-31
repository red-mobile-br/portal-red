namespace RedMobilePedidos.API.Models.Report;

public sealed record RelatorioPedidoModel
{
    public required string IdPedido { get; init; }
    public required string TipoPedido { get; init; }
    public required string TextoStatus { get; init; }
    public required string CorStatus { get; init; }
    public required DateTime DataLancamento { get; init; }
    public required string NomeRepresentante { get; init; }
    public required RelatorioClientePedidoModel Cliente { get; init; }
    public required string ModoFrete { get; init; }
    public required bool TemDadosAgendamento { get; init; }
    public RelatorioAgendamentoPedidoModel? DadosAgendamento { get; init; }

    public required bool TemEnderecoEntrega { get; init; }
    public RelatorioEnderecoPedidoModel? EnderecoEntrega { get; init; }

    public required List<RelatorioProdutoPedidoModel> Produtos { get; init; }

    public required string NumeroPedidoCompra { get; init; } = "-";
    public required string PlanoPagamento { get; init; }
    public required DateTime DataEmissao { get; init; }

    public required int TotalProdutos { get; init; }
    public required int TotalVolumes { get; init; }
    public required decimal TotalPesoLiquido { get; init; }
    public required decimal TotalPesoBruto { get; init; }

    public required decimal ValorProdutos { get; init; }
    public required decimal ValorICMS { get; init; }
    public required decimal ValorICMSST { get; init; }
    public required decimal ValorIPI { get; init; }
    public required decimal ValorNota { get; init; }

    public required bool TemNotas { get; init; }
    public required string? InformacoesNota { get; init; }
    public required string? ObservacoesPedido { get; init; }
}

public sealed record RelatorioClientePedidoModel
{
    public required string Id { get; init; }
    public required string RazaoSocial { get; init; }
    public required string NomeFantasia { get; init; }
    public required string Cnpj { get; init; }
    public required RelatorioEnderecoPedidoModel Endereco { get; init; }
}

public sealed record RelatorioEnderecoPedidoModel
{
    public required string Cep { get; init; }
    public required string Logradouro { get; init; }
    public required string Numero { get; init; }
    public required string? Complemento { get; init; }
    public required string Bairro { get; init; }
    public required string Cidade { get; init; }
    public required string Estado { get; init; }
}

public sealed record RelatorioAgendamentoPedidoModel
{
    public required string Email { get; init; }
    public required string Telefone { get; init; }
}

public sealed record RelatorioProdutoPedidoModel
{
    public required int Indice { get; init; }
    public required string Id { get; init; }
    public required string Descricao { get; init; }
    public required int Quantidade { get; init; }
    public required decimal ValorUnitario { get; init; }
    public required decimal PercentualIPI { get; init; }
    public required decimal PercentualICMS { get; init; }
    public required decimal PercentualICMSST { get; init; }
    public required decimal ValorTotal { get; init; }
}
