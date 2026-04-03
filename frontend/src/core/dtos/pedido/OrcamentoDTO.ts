import PedidoDTO from './PedidoDTO';

export default interface OrcamentoDTO extends PedidoDTO {
    dataImplementacao?: string;
    dataCancelamento?: string;
}
