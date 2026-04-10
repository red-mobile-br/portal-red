import ImpostoProdutoDTO from './ImpostoProdutoDTO';

export default interface RespostaImpostoLoteDTO {
    itens?: ImpostoProdutoDTO[];
    totalProdutos?: number;
    totalIcms?: number;
    totalIpi?: number;
    totalIcmsSt?: number;
    totalNota?: number;
    pesoLiquido?: number;
    pesoBruto?: number;
    margemPedido?: number;
    totalVolumes?: number;
    totalItens?: number;
}
