import ImpostoProdutoDTO from './ImpostoProdutoDTO';

export default interface RespostaImpostoLoteDTO {
    itens?: ImpostoProdutoDTO[];
    totalProdutos?: number;
    totalICMS?: number;
    totalIPI?: number;
    totalICMSST?: number;
    totalNF?: number;
    pesoLiquido?: number;
    pesoBruto?: number;
    margemPedido?: number;
    totalVolumes?: number;
    totalItens?: number;
}
