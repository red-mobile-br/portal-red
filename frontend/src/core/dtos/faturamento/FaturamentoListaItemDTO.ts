export default interface FaturamentoListaItemDTO {
    pedido?: string;
    cliente?: string;
    dataHora?: string;
    valorPedido?: number;
    notaFiscal?: string;
    representante?: string;
    nomeRepresentante?: string;
    valorBase?: number;
}
