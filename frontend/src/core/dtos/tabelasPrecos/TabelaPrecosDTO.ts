import ProdutoTabelaPrecosDTO from "./ProdutoTabelaPrecosDTO";

export default interface TabelaPrecosDTO {
    valorMinimoFreteCif?: number;
    comentarios?: string;
    dataAtualizacao?: string;
    prazo?: string;
    descricao?: string;
    icms?: number;
    produtos?: ProdutoTabelaPrecosDTO[];
}
