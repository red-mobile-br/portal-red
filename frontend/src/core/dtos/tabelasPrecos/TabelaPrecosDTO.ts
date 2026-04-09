import ProdutoTabelaPrecosDTO from "./ProdutoTabelaPrecosDTO";

export default interface TabelaPrecosDTO {
    valorMinimoFreteCif?: number;
    observacoes?: string;
    dataAtualizacao?: string;
    dataValidade?: string;
    descricao?: string;
    icms?: number;
    produtos?: ProdutoTabelaPrecosDTO[];
}
