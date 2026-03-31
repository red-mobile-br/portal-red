import ItemGraficoDTO from "../ItemGraficoDTO";

export interface DashboardDTO {
    totalPedidosPeriodo: number;
    totalComissaoPeriodo: number;
    pedidosAbertosPeriodo: number;
    comissoesPeriodo: ItemGraficoDTO[];
    clientesPorEstado: {
        estado: string;
        quantidadeClientes: number
    }[];
}
