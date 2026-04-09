export default interface ComissaoListaItemDTO {
    numeroPedido?: string;
    nomeCliente?: string;
    numeroTitulo?: string;
    parcela?: number;
    dataVencimento?: string;
    dataBaixa?: string;
    valorTitulo?: number;
    valorBase?: number;
    percentualComissao?: number;
    idRepresentante?: string;
    nomeRepresentante?: string;
    valorComissao?: number;
}
