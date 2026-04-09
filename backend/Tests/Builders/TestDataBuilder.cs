using System.Collections.Generic;
using Bogus;
using RedMobilePedidos.API.Models.Responses.Clientes;
using RedMobilePedidos.API.Models.Responses.Common;
using RedMobilePedidos.API.Models.Responses.Pedidos;
using RedMobilePedidos.API.Models.Responses.Produtos;
using RedMobilePedidos.API.Models.Responses.Metas;
using RedMobilePedidos.API.Models.Responses.Comissao;
using RedMobilePedidos.API.Models.Responses.DashboardFaturamentos;
using RedMobilePedidos.API.Enums;

namespace RedMobilePedidos.Tests.Builders;

public static class TestDataBuilder
{
    private static readonly Faker Faker = new("pt_BR");

    public static Faker<ClienteCompletoDto> ClientBuilder()
    {
        return new Faker<ClienteCompletoDto>()
            .CustomInstantiator(f => new ClienteCompletoDto
            {
                Id = f.Random.AlphaNumeric(6),
                IdRepresentante = f.Random.AlphaNumeric(6),
                NomeRepresentante = f.Name.FullName(),
                IdGerente = f.Random.AlphaNumeric(6),
                NomeGerente = f.Name.FullName(),
                Cnpj = f.Random.ReplaceNumbers("##.###.###/####-##"),
                RazaoSocial = f.Company.CompanyName(),
                NomeFantasia = f.Company.CompanyName(),
                InscricaoEstadual = f.Random.Number(100000000, 999999999).ToString(),
                Email = f.Internet.Email(),
                Telefone = f.Phone.PhoneNumber(),
                Endereco = new Endereco
                {
                    Logradouro = f.Address.StreetName(),
                    Numero = f.Random.Int(1, 9999),
                    Complemento = f.Address.SecondaryAddress(),
                    Bairro = f.Address.County(),
                    Cidade = f.Address.City(),
                    Uf = f.Address.StateAbbr(),
                    Cep = f.Address.ZipCode()
                }
            });
    }

    public static Faker<PedidoDetalhado> OrderBuilder()
    {
        return new Faker<PedidoDetalhado>()
            .RuleFor(o => o.Id, f => f.Random.AlphaNumeric(6))
            .RuleFor(o => o.Cliente, f => new ClientePedido
            {
                Id = f.Random.AlphaNumeric(6),
                RazaoSocial = f.Company.CompanyName(),
                NomeFantasia = f.Company.CompanyName(),
                Cnpj = f.Random.ReplaceNumbers("##.###.###/####-##"),
                Endereco = new Endereco
                {
                    Logradouro = f.Address.StreetName(),
                    Numero = f.Random.Int(1, 9999),
                    Complemento = f.Address.SecondaryAddress(),
                    Bairro = f.Address.County(),
                    Cidade = f.Address.City(),
                    Uf = f.Address.StateAbbr(),
                    Cep = f.Address.ZipCode()
                }
            })
            .RuleFor(o => o.ValorNota, f => f.Finance.Amount(100, 50000))
            .RuleFor(o => o.DataEmissao, f => f.Date.Past(1))
            .RuleFor(o => o.Status, f => f.Random.Int(0, 2).ToString())
            .RuleFor(o => o.TipoPedido, f => f.PickRandom<TipoPedido>())
            .RuleFor(o => o.PlanoPagamento, f => f.Random.AlphaNumeric(3))
            .RuleFor(o => o.IdRepresentante, f => f.Random.AlphaNumeric(6))
            .RuleFor(o => o.NomeRepresentante, f => f.Name.FullName())
            .RuleFor(o => o.Anotacoes, f => [])
            .RuleFor(o => o.NotasFiscais, f => [])
            .RuleFor(o => o.Produtos, f => new List<ProdutoPedido>
            {
                new()
                {
                    Ordem = 1,
                    Id = f.Commerce.Ean13(),
                    Descricao = f.Commerce.ProductName(),
                    Quantidade = f.Random.Int(1, 10),
                    PercentualDesconto = f.Finance.Amount(0, 20),
                    ValorUnitario = f.Finance.Amount(10, 1000),
                    PrecoBase = f.Finance.Amount(10, 1000),
                    Comissao = f.Finance.Amount(0, 5),
                    ValorTotal = f.Finance.Amount(10, 10000),
                    Margem = f.Finance.Amount(0, 30),
                    PercentualICMS = f.Finance.Amount(0, 18),
                    PercentualIPI = f.Finance.Amount(0, 15),
                    PercentualICMSST = f.Finance.Amount(0, 5),
                    SaldoPendente = 0,
                    ICMS = f.Finance.Amount(0, 200),
                    ICMSST = f.Finance.Amount(0, 50),
                    IPI = f.Finance.Amount(0, 100),
                    ComissaoMaxima = f.Finance.Amount(3, 10)
                }
            })
            .RuleFor(o => o.EnderecoEntrega, f => new Endereco
            {
                Logradouro = f.Address.StreetName(),
                Numero = f.Random.Int(1, 9999),
                Complemento = f.Address.SecondaryAddress(),
                Bairro = f.Address.County(),
                Cidade = f.Address.City(),
                Uf = f.Address.StateAbbr(),
                Cep = f.Address.ZipCode()
            })
            .RuleFor(o => o.PesoLiquido, f => f.Finance.Amount(0, 500))
            .RuleFor(o => o.PesoBruto, f => f.Finance.Amount(0, 600));
    }

    public static Faker<Produto> ProductBuilder()
    {
        return new Faker<Produto>()
            .RuleFor(p => p.Id, f => f.Commerce.Ean13())
            .RuleFor(p => p.Descricao, f => f.Commerce.ProductName())
            .RuleFor(p => p.PrecoBase, f => f.Finance.Amount(10, 5000))
            .RuleFor(p => p.PercentualIPI, f => f.Finance.Amount(0, 15))
            .RuleFor(p => p.PercentualICMS, f => f.Finance.Amount(0, 18))
            .RuleFor(p => p.ValorICMSST, f => f.Finance.Amount(0, 100))
            .RuleFor(p => p.PesoLiquido, f => f.Finance.Amount(0, 100))
            .RuleFor(p => p.PesoBruto, f => f.Finance.Amount(0, 120));
    }

    public static Faker<Meta> MetaBuilder()
    {
        return new Faker<Meta>()
            .RuleFor(g => g.Titulo, f => f.Commerce.ProductAdjective() + " Meta")
            .RuleFor(g => g.DataInicio, f => f.Date.Past(1))
            .RuleFor(g => g.DataFim, f => f.Date.Future(1))
            .RuleFor(g => g.ValorMeta, f => f.Finance.Amount(10000, 100000))
            .RuleFor(g => g.ValorRealizado, f => f.Finance.Amount(0, 80000))
            .RuleFor(g => g.Status, f => f.PickRandom<StatusMeta>())
            .RuleFor(g => g.IdRepresentante, f => f.Random.AlphaNumeric(6))
            .RuleFor(g => g.NomeRepresentante, f => f.Name.FullName());
    }

    public static Faker<RespostaPaginada<T>> PageResponseBuilder<T>(List<T> data) where T : class
    {
        return new Faker<RespostaPaginada<T>>()
            .RuleFor(p => p.Dados, f => data)
            .RuleFor(p => p.NumeroPagina, f => 1)
            .RuleFor(p => p.TotalPaginas, f => 1);
    }

    public static Faker<RespostaPadrao> DefaultResponseBuilder(bool success = true)
    {
        return new Faker<RespostaPadrao>()
            .RuleFor(r => r.Sucesso, f => success)
            .RuleFor(r => r.Detalhes, f => success ? "Operação realizada com sucesso" : "Erro ao realizar operação");
    }

    public static Faker<ComissaoListaItem> CommissionBuilder()
    {
        return new Faker<ComissaoListaItem>()
            .RuleFor(c => c.NumeroPedido, f => f.Random.AlphaNumeric(6))
            .RuleFor(c => c.NomeCliente, f => f.Company.CompanyName())
            .RuleFor(c => c.NumeroTitulo, f => f.Commerce.ProductName())
            .RuleFor(c => c.Parcela, f => f.Random.Int(1, 12))
            .RuleFor(c => c.DataVencimento, f => f.Date.Future(3))
            .RuleFor(c => c.DataBaixa, f => f.Date.Future(6))
            .RuleFor(c => c.ValorTitulo, f => f.Finance.Amount(100, 5000))
            .RuleFor(c => c.ValorBase, f => f.Finance.Amount(100, 5000))
            .RuleFor(c => c.PercentualComissao, f => f.Finance.Amount(1, 10))
            .RuleFor(c => c.ValorComissao, (f, c) => c.ValorBase * (c.PercentualComissao / 100))
            .RuleFor(c => c.IdRepresentante, f => f.Random.AlphaNumeric(6))
            .RuleFor(c => c.NomeRepresentante, f => f.Name.FullName());
    }

    public static Faker<FaturamentoItem> DashboardFaturamentoBuilder()
    {
        return new Faker<FaturamentoItem>()
            .RuleFor(r => r.NumeroPedido, f => f.Random.AlphaNumeric(6))
            .RuleFor(r => r.NomeCliente, f => f.Company.CompanyName())
            .RuleFor(r => r.ValorBruto, f => f.Finance.Amount(100, 10000))
            .RuleFor(r => r.ValorBase, f => f.Finance.Amount(100, 10000))
            .RuleFor(r => r.DataEmissao, f => f.Date.Past(3))
            .RuleFor(r => r.IdRepresentante, f => f.Random.AlphaNumeric(6))
            .RuleFor(r => r.NomeRepresentante, f => f.Name.FullName())
            .RuleFor(r => r.NumeroNota, f => f.Random.AlphaNumeric(8));
    }
}
