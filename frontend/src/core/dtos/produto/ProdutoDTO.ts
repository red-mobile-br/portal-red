export default interface ProdutoDTO {
    precoBase: number
    descricao: string;
    icms: number;
    icmsst: number;
    id: string
    ipi: number
    margem: number;
    percentualICMS: number;
    percentualIPI: number;
    percentualICMSST: number;
    saldoPendente: number;

    // Valores injetados
    quantidade: number;
    percentualDesconto: number;
    comissao: number
    valorUnitario: number;
    valorTotal: number
    comissaoMaxima: number;
}
