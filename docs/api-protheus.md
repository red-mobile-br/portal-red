# Documentação da API Protheus — Portal RedMobile

> **Base URL:** `http://187.45.183.238:8090/rest`
> **Autenticação:** Basic Auth em todas as requisições
> **Parâmetro obrigatório em todas as rotas protegidas:** `UsuarioLogado` (query string)

---

## Parâmetros de Filtro Comuns (Query String)

Usados na maioria dos endpoints de listagem e dashboard:

| Parâmetro | Tipo | Descrição |
|---|---|---|
| `UsuarioLogado` | string | ID do representante logado (obrigatório) |
| `DataDe` | string (yyyyMMdd) | Filtro de data inicial |
| `DataAte` | string (yyyyMMdd) | Filtro de data final |
| `Pagina` | int | Número da página |
| `Tamanho` | int | Quantidade de itens por página |
| `OrdenarPor` | string | Campo de ordenação |
| `Direcao` | string | Direção da ordenação |
| `Busca` | string | Texto de busca livre |
| `Status` | string | Filtro por status |
| `Cnpj` | string | Filtro por CNPJ |
| `FiltroUsuario` | string | Filtro por usuário |
| `IdRepresentante` | string | Filtro por representante |
| `IdGerente` | string | Filtro por gerente |
| `IdCliente` | string | Filtro por cliente |

---

## 1. Login (LoginController)

| Método | Endpoint | Descrição |
|---|---|---|
| `POST` | `/rest/Login` | Autentica o usuário |
| `GET` | `/rest/DadosUsuario/{nomeUsuario}` | Busca dados do usuário após login |

### POST `/rest/Login` — Body (JSON)
| Atributo | Tipo | Obrigatório |
|---|---|---|
| `NomeUsuario` | string | Sim |
| `Senha` | string | Sim |

---

## 2. Cliente (ClienteController)

| Método | Endpoint | Parâmetros |
|---|---|---|
| `GET` | `/rest/Cliente` | Filtros comuns (query string) |
| `GET` | `/rest/Cliente/{cnpj}` | `UsuarioLogado` |
| `GET` | `/rest/Cliente/{cnpj}/pedidos` | `UsuarioLogado` |
| `GET` | `/rest/Dashboard/Cliente` | Filtros comuns (query string) |
| `POST` | `/rest/Cliente` | `UsuarioLogado` (query) + body JSON abaixo |

### POST `/rest/Cliente` — Body (JSON)
| Atributo | Tipo | Obrigatório |
|---|---|---|
| `UsuarioLogado` | string | Sim |
| `RazaoSocial` | string | Não |
| `NomeFantasia` | string | Não |
| `Cnpj` | string | Não |
| `InscricaoEstadual` | string | Não |
| `Suframa` | string | Não |
| `DataFundacao` | string | Não |
| `CNAE` | string | Não |
| `RamoAtividade` | string | Não |
| `Telefone` | string | Não |
| `TelefoneCobranca` | string | Não |
| `Fax` | string | Não |
| `Celular` | string | Não |
| `NomeContato` | string | Não |
| `Email` | string | Não |
| `Website` | string | Não |
| `CapitalSocial` | decimal | Não |
| `IdRepresentante` | string | Não |
| `IdGerente` | string | Não |
| `Endereco` | objeto `Endereco` | Não |
| `EnderecoEntrega` | objeto `Endereco` | Não |
| `TelefoneEntrega` | string | Não |
| `EmailEntrega` | string | Não |
| `NomeContatoEntrega` | string | Não |
| `Socios` | array de `SocioInfo` | Não |
| `ReferenciasComerciais` | array de `ReferenciaComercial` | Não |
| `DadosBancarios` | array de `DadosBancarios` | Não |
| `ContratoSocial` | objeto `ArquivoInfo` (`Nome`, `ConteudoBase64`) | Não |
| `DocumentoSintegra` | objeto `ArquivoInfo` (`Nome`, `ConteudoBase64`) | Não |
| `Comprovantes` | array de `ArquivoInfo` | Não |
| `NotasComerciais` | array de `ArquivoInfo` | Não |

---

## 3. Produto (ClienteController)

| Método | Endpoint | Parâmetros |
|---|---|---|
| `GET` | `/rest/Produto` | `IdCliente` + filtros comuns |
| `GET` | `/rest/Produto/{codigoProduto}` | `IdCliente` |
| `GET` | `/rest/Produto` (cálculo de imposto unitário) | Query string abaixo |
| `POST` | `/rest/itens/impostos` | Body JSON abaixo |

### GET `/rest/Produto` — Cálculo de Imposto Unitário (Query String)
| Parâmetro | Tipo | Obrigatório |
|---|---|---|
| `CodigoProduto` | string | Sim |
| `IdCliente` | string | Sim |
| `UsuarioLogado` | string | Sim |
| `PrecoBase` | decimal | Sim |
| `PrecoVenda` | decimal | Sim |
| `Quantidade` | int | Sim |
| `TipoPedido` | int | Sim |
| `PlanoPagamento` | string | Não |
| `Comissao` | decimal | Não |
| `IdPedido` | string | Não |
| `IdRepresentante` | string | Não |
| `IdGerente` | string | Não |
| `ModoFrete` | int | Não |

### POST `/rest/itens/impostos` — Cálculo de Imposto em Lote (Body JSON)
| Atributo | Tipo | Obrigatório |
|---|---|---|
| `IdCliente` | string | Sim |
| `UsuarioLogado` | string | Sim |
| `TipoPedido` | int | Não |
| `PlanoPagamento` | string | Não |
| `IdRepresentante` | string | Não |
| `IdGerente` | string | Não |
| `ModoFrete` | int | Não |
| `IdPedido` | string | Não |
| `Itens` | array de `ItemImpostoLote` | Sim |

**Item do array `Itens`:**
| Atributo | Tipo | Obrigatório |
|---|---|---|
| `IdProduto` | string | Sim |
| `PrecoBase` | decimal | Não |
| `PrecoVenda` | decimal | Não |
| `Quantidade` | int | Não |
| `Comissao` | decimal | Não |

---

## 4. Pedido (PedidoController)

| Método | Endpoint | Parâmetros |
|---|---|---|
| `GET` | `/rest/Pedido` | `UsuarioLogado` + filtros comuns |
| `GET` | `/rest/Pedido/{numero}` | `UsuarioLogado` |
| `POST` | `/rest/Pedido` | `UsuarioLogado` + body `CriarPedido` |
| `PUT` | `/rest/Pedido/{numero}` | `UsuarioLogado` + body `CriarPedido` |
| `DELETE` | `/rest/Pedido/{numero}` | `UsuarioLogado` |
| `POST` | `/rest/Pedido/{numero}/aprovar` | `UsuarioLogado` |
| `POST` | `/rest/Pedido/{numero}/recusar` | `UsuarioLogado` |
| `GET` | `/rest/Pedido/{numero}/notaFiscal/{id}` | `UsuarioLogado` |
| `GET` | `/rest/Pedido/{numero}/boleto/{id}` | `UsuarioLogado` |
| `GET` | `/rest/Dashboard/Pedido` | `UsuarioLogado` + filtros comuns |

### Body `CriarPedido` (POST e PUT)
| Atributo | Tipo | Obrigatório |
|---|---|---|
| `IdCliente` | string | Sim |
| `ModoFrete` | int (enum) | Sim |
| `EnderecoEntrega` | objeto `Endereco` | Sim |
| `Produtos` | array de `CriarPedidoProduto` | Sim |
| `DataEmissao` | datetime | Sim |
| `TipoPedido` | int (enum) | Sim |
| `PlanoPagamento` | string | Não |
| `NumeroPedidoCompra` | string | Não |
| `InformacoesNota` | string | Não |
| `ObservacoesPedido` | string | Não |
| `IdRepresentante` | string | Não |
| `IdGerente` | string | Não |
| `DadosAgendamento` | objeto `DadosAgendamento` | Não |

**Item do array `Produtos`:**
| Atributo | Tipo | Obrigatório |
|---|---|---|
| `Id` | string | Sim |
| `Quantidade` | int | Sim |
| `ValorUnitario` | decimal | Sim |
| `Comissao` | decimal | Sim |

---

## 5. Orçamento (OrcamentoController)

| Método | Endpoint | Parâmetros |
|---|---|---|
| `GET` | `/rest/Orcamento` | `UsuarioLogado` + filtros comuns |
| `GET` | `/rest/Orcamento/{numero}` | `UsuarioLogado` |
| `POST` | `/rest/Orcamento` | `UsuarioLogado` + body `CriarPedido` (mesmo schema do Pedido) |
| `PUT` | `/rest/Orcamento/{numero}` | `UsuarioLogado` + body `CriarPedido` |
| `DELETE` | `/rest/Orcamento/{numero}` | `UsuarioLogado` |
| `POST` | `/rest/Orcamento/{numero}/aprovar` | `UsuarioLogado` |
| `POST` | `/rest/Orcamento/{numero}/recusar` | `UsuarioLogado` |
| `GET` | `/rest/Dashboard/Orcamento` | `UsuarioLogado` + filtros comuns |

---

## 6. Título (TituloController)

| Método | Endpoint | Parâmetros |
|---|---|---|
| `GET` | `/rest/Titulo` | `UsuarioLogado` + filtros comuns |
| `GET` | `/rest/Dashboard/Titulo` | `UsuarioLogado` + filtros comuns |

---

## 7. Comissão (ComissaoController)

| Método | Endpoint | Parâmetros |
|---|---|---|
| `GET` | `/rest/Comissao` | `UsuarioLogado` + `Status` (obrigatório: `1` ou `2`) + filtros comuns |
| `GET` | `/rest/Dashboard/Comissao` | `UsuarioLogado` + `Status` (obrigatório: `0`, `1` ou `2`) + filtros comuns |

---

## 8. Meta (MetaController)

| Método | Endpoint | Parâmetros |
|---|---|---|
| `GET` | `/rest/Meta` | `UsuarioLogado` + `DataDe` + `DataAte` + filtros comuns |

> **Nota:** usado tanto para metas vigentes (período do mês atual por padrão) quanto para histórico.

---

## 9. Faturamento (FaturamentoController)

| Método | Endpoint | Parâmetros |
|---|---|---|
| `GET` | `/rest/Faturamento` | `UsuarioLogado` + filtros comuns |
| `GET` | `/rest/Dashboard/Faturamento` | `UsuarioLogado` + filtros comuns |

---

## 10. Gerentes (GerentesController)

| Método | Endpoint | Parâmetros |
|---|---|---|
| `GET` | `/rest/Gerentes` | `UsuarioLogado` + filtros comuns |

---

## 11. Representantes (RepresentantesController)

| Método | Endpoint | Parâmetros |
|---|---|---|
| `GET` | `/rest/Representantes` | `UsuarioLogado` + filtros comuns |

---

## 12. Plano de Pagamento (PlanoPagamentoController)

| Método | Endpoint | Parâmetros |
|---|---|---|
| `GET` | `/rest/PlanoPagamento` | Nenhum |

---

## 13. Tabela de Preços (TabelaPrecosController)

| Método | Endpoint | Parâmetros |
|---|---|---|
| `GET` | `/rest/TabelaPrecos` | `UF` (sigla do estado, ex: `SP`) |
