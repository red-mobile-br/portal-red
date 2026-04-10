export default interface ProdutoDTO {
    precoBase?: number;
    descricao?: string;
    id?: string;
    margem?: number;
    percentualICMS?: number;
    percentualIPI?: number;
    percentualICMSST?: number;
    saldoPendente?: number;

    // Valores injetados
    quantidade?: number;
    percentualDesconto?: number;
    comissao?: number;
    valorUnitario?: number;
    valorTotal?: number;
    comissaoMaxima?: number;
}
