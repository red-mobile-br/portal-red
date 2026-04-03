export default interface ComissaoListaItemDTO {
    pedido?: string;
    nomeCliente?: string;
    cnpj?: string;
    titulo?: string;
    parcela?: number;
    dataVencimento?: string;
    dataBaixa?: string;
    valorTitulo?: number;
    valorBase?: number;
    percentualComissao?: number;
    representante?: string;
    nomeRepresentante?: string;
    valorComissao?: number;
}
