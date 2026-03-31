import FiltroPadraoQuery from "./FiltroPadraoQuery";

export default interface ProdutoQuery extends FiltroPadraoQuery {
    busca: string;
}