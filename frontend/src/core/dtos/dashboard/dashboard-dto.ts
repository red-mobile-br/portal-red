import ItemGraficoDTO from "../ItemGraficoDTO";

export interface DashboardDTO {
    totalPedidosNoPeriodo?: number;
    totalComissaoPeriodo?: number;
    pedidosAbertosNoPeriodo?: number;
    comissoesPeriodo?: ItemGraficoDTO[];
    clientesPorEstado?: {
        uf?: string;
        quantidadeClientes?: number;
    }[];
}
