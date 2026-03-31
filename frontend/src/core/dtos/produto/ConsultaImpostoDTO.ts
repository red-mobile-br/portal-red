export default interface ConsultaImpostoDTO {
    precoBase: number;
    precoVenda: number;
    quantidade: number;
    tipoPedido: number;
    planoPagamento: string;
    comissao: number;
    idPedido?: string;
    idCliente: string;
    idRepresentante: string | null;
    idGerente: string | null;
    modoFrete: 0 | 1;
}
