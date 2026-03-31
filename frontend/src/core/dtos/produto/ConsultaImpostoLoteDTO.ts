export interface ItemConsultaImpostoLoteDTO {
    idProduto: string;
    precoBase: number;
    precoVenda: number;
    quantidade: number;
    comissao: number;
}

export default interface ConsultaImpostoLoteDTO {
    idCliente: string;
    idRepresentante: string | null;
    idGerente: string | null;
    tipoPedido: number;
    planoPagamento: string;
    modoFrete: 0 | 1;
    idPedido?: string;
    itens: ItemConsultaImpostoLoteDTO[];
}
