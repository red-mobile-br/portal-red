import ItemGraficoDTO from "../ItemGraficoDTO";
import MelhorClienteDTO from "./MelhorClienteDTO";

export default interface DashboardClienteDTO {
    clientesPorEstado?: {
        estado?: string;
        quantidadeClientes?: number;
    }[];
    clientesPorEstatus?: ItemGraficoDTO[];
    melhoresClientesPorPedidos?: MelhorClienteDTO[];
    melhoresClientesPorFaturamento?: MelhorClienteDTO[];
}
