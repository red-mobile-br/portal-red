import ComissaoTabelaPrecosDTO from "./ComissaoTabelaPrecosDTO";

export default interface ProdutoTabelaPrecosDTO {
    referenciaComercial: string;
    descricao: string;
    imagem: string;
    comissaoPorcento3: ComissaoTabelaPrecosDTO;
    comissaoPorcento5: ComissaoTabelaPrecosDTO;
    precoMercado: number;
    percentualIPI: number;
}
