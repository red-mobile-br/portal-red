import ItemGraficoDTO from "../ItemGraficoDTO";
import MelhorClienteDTO from "./MelhorClienteDTO";

export default interface DashboardClienteDTO {
    clientesPorEstado?: {
        uf?: string;
        quantidadeClientes?: number;
    }[];
    clientesPorStatus?: ItemGraficoDTO[];
    melhoresClientesPorPedidos?: MelhorClienteDTO[];
    melhoresClientesPorFaturamento?: MelhorClienteDTO[];
}
