using RedMobilePedidos.API.Helpers;
using RedMobilePedidos.API.Models.Report;
using RedMobilePedidos.API.Models.Responses.Pedidos;
using RedMobilePedidos.API.Enums;

namespace RedMobilePedidos.API.Services;

internal sealed class RelatorioPedidoService(IEmailTemplateService templateService) : IRelatorioPedidoService
{
    public string GerarRelatorioHtml(PedidoDetalhado pedido)
    {
        var modelo = MapearParaModeloRelatorio(pedido);
        return templateService.RenderizarTemplate("Report", "RelatorioPedido", modelo);
    }

    private static RelatorioPedidoModel MapearParaModeloRelatorio(PedidoDetalhado pedido)
    {
        var statusInfo = ParserStatusPedido.ObterStatus(pedido.Status);
        var totalVolumes = pedido.Produtos?.Sum(p => p.Quantidade ?? 0) ?? 0;
        var totalPesoLiquido = pedido.PesoLiquido ?? 0;
        var totalPesoBruto = pedido.PesoBruto ?? 0;

        return new RelatorioPedidoModel
        {
            IdPedido = pedido.Id ?? string.Empty,
            TipoPedido = ObterTipoPedido(pedido.TipoPedido ?? Enums.TipoPedido.Venda),
            TextoStatus = statusInfo.Titulo,
            CorStatus = statusInfo.Cor,
            DataLancamento = pedido.DataLancamento ?? DateTime.MinValue,
            NomeRepresentante = pedido.NomeRepresentante ?? string.Empty,

            Cliente = new RelatorioClientePedidoModel
            {
                Id = pedido.Cliente?.Id ?? string.Empty,
                RazaoSocial = pedido.Cliente?.RazaoSocial ?? string.Empty,
                NomeFantasia = pedido.Cliente?.NomeFantasia ?? string.Empty,
                Cnpj = FormatarCnpj(pedido.Cliente?.Cnpj ?? string.Empty),
                Endereco = new RelatorioEnderecoPedidoModel
                {
                    Cep = pedido.Cliente?.Endereco?.Cep ?? string.Empty,
                    Logradouro = pedido.Cliente?.Endereco?.Logradouro ?? string.Empty,
                    Numero = pedido.Cliente?.Endereco?.Numero ?? string.Empty,
                    Complemento = pedido.Cliente?.Endereco?.Complemento,
                    Bairro = pedido.Cliente?.Endereco?.Bairro ?? string.Empty,
                    Cidade = pedido.Cliente?.Endereco?.Cidade ?? string.Empty,
                    Estado = pedido.Cliente?.Endereco?.Estado ?? string.Empty
                }
            },

            ModoFrete = ObterModoFrete(pedido.ModoFrete ?? Enums.ModoFrete.CIF),
            TemDadosAgendamento = pedido.DadosAgendamento != null && !string.IsNullOrEmpty(pedido.DadosAgendamento.Email),
            DadosAgendamento = pedido.DadosAgendamento != null ? new RelatorioAgendamentoPedidoModel
            {
                Email = pedido.DadosAgendamento.Email ?? string.Empty,
                Telefone = FormatarTelefone(pedido.DadosAgendamento.Telefone ?? string.Empty)
            } : null,

            TemEnderecoEntrega = pedido.EnderecoEntrega != null && !string.IsNullOrEmpty(pedido.EnderecoEntrega.Cep),
            EnderecoEntrega = pedido.EnderecoEntrega != null ? new RelatorioEnderecoPedidoModel
            {
                Cep = pedido.EnderecoEntrega.Cep ?? string.Empty,
                Logradouro = pedido.EnderecoEntrega.Logradouro ?? string.Empty,
                Numero = pedido.EnderecoEntrega.Numero ?? string.Empty,
                Complemento = pedido.EnderecoEntrega.Complemento,
                Bairro = pedido.EnderecoEntrega.Bairro ?? string.Empty,
                Cidade = pedido.EnderecoEntrega.Cidade ?? string.Empty,
                Estado = pedido.EnderecoEntrega.Estado ?? string.Empty
            } : null,

            Produtos = (pedido.Produtos ?? []).Select((p, indice) => new RelatorioProdutoPedidoModel
            {
                Indice = indice + 1,
                Id = p.Id ?? string.Empty,
                Descricao = p.Descricao ?? string.Empty,
                Quantidade = p.Quantidade ?? 0,
                ValorUnitario = p.ValorUnitario ?? 0,
                PercentualIPI = p.PercentualIPI ?? 0,
                PercentualICMS = p.PercentualICMS ?? 0,
                PercentualICMSST = p.PercentualICMSST ?? 0,
                ValorTotal = p.ValorTotal ?? 0
            }).ToList(),

            NumeroPedidoCompra = string.IsNullOrEmpty(pedido.NumeroPedidoCompra) ? "-" : pedido.NumeroPedidoCompra,
            PlanoPagamento = pedido.PlanoPagamento ?? string.Empty,
            DataEmissao = pedido.DataEmissao ?? DateTime.MinValue,

            TotalProdutos = pedido.Produtos?.Count ?? 0,
            TotalVolumes = totalVolumes,
            TotalPesoLiquido = totalPesoLiquido,
            TotalPesoBruto = totalPesoBruto,

            ValorProdutos = pedido.ValorProdutos ?? 0,
            ValorICMS = pedido.ValorICMS ?? 0,
            ValorICMSST = pedido.ValorICMSST ?? 0,
            ValorIPI = pedido.ValorIPI ?? 0,
            ValorNota = pedido.ValorNota ?? 0,

            TemNotas = !string.IsNullOrEmpty(pedido.InformacoesNota) || !string.IsNullOrEmpty(pedido.ObservacoesPedido),
            InformacoesNota = pedido.InformacoesNota,
            ObservacoesPedido = pedido.ObservacoesPedido
        };
    }

    private static string FormatarCnpj(string cnpj)
    {
        if (string.IsNullOrEmpty(cnpj) || cnpj.Length != 14)
        {
            return cnpj;
        }

        return $"{cnpj[..2]}.{cnpj[2..5]}.{cnpj[5..8]}/{cnpj[8..12]}-{cnpj[12..14]}";
    }

    private static string FormatarTelefone(string telefone)
    {
        if (string.IsNullOrEmpty(telefone))
        {
            return telefone;
        }

        telefone = new string(telefone.Where(char.IsDigit).ToArray());

        return telefone.Length switch
        {
            11 => $"({telefone[..2]}) {telefone[2..7]}-{telefone[7..11]}",
            10 => $"({telefone[..2]}) {telefone[2..6]}-{telefone[6..10]}",
            _ => telefone
        };
    }

    private static string ObterTipoPedido(TipoPedido tipoPedido)
    {
        return tipoPedido switch
        {
            TipoPedido.Venda => "Venda",
            TipoPedido.Bonificacao => "Bonificação",
            _ => "Desconhecido"
        };
    }

    private static string ObterModoFrete(ModoFrete modo)
    {
        return modo switch
        {
            ModoFrete.CIF => "CIF - Pago pelo Remetente",
            ModoFrete.FOB => "FOB - Pago pelo Destinatário",
            _ => "Não especificado"
        };
    }
}
