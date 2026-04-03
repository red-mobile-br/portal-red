import PedidoListaItemDTO from './PedidoListaItemDTO';

export default interface OrcamentoListaItemDTO extends PedidoListaItemDTO {
    idPedido?: string;
    dataImplementacao?: string;
    dataCancelamento?: string;
}
