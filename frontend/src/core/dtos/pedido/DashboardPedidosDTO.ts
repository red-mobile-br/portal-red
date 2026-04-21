import ItemGraficoDTO from "../ItemGraficoDTO";

export default interface DashboardPedidosDTO {
    totalPedidosNoPeriodo?: number;
    pedidosAbertosNoPeriodo?: number;
    valorTotalPedidos?: number;
    pedidosPorPeriodo?: ItemGraficoDTO[];
    pedidosPorTipo?: ItemGraficoDTO[];
}
