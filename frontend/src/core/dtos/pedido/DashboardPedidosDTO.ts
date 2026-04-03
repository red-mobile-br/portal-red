import ItemGraficoDTO from "../ItemGraficoDTO";

export default interface DashboardPedidosDTO {
    totalPedidosPeriodo?: number;
    pedidosAbertosPeriodo?: number;
    valorTotalPedidos?: number;
    pedidosPorPeriodo?: ItemGraficoDTO[];
    pedidosPorTipo?: ItemGraficoDTO[];
}
