import RespostaPaginadaDTO from "../core/dtos/RespostaPaginadaDTO";
import DashboardFaturamentoDTO from "../core/dtos/faturamento/DashboardFaturamentoDTO";
import FaturamentoListaItemDTO from "../core/dtos/faturamento/FaturamentoListaItemDTO";
import FiltroPadraoQuery from "../core/query-objects/FiltroPadraoQuery";
import { obter } from "./base-service";

const faturamentoService = {
    obterDashboardFaturamentos: (query: FiltroPadraoQuery) => {
        return obter<DashboardFaturamentoDTO>('/faturamento/dashboard', query);
    },
    obterFaturamentos: (query: FiltroPadraoQuery) => {
        return obter<RespostaPaginadaDTO<FaturamentoListaItemDTO>>('/faturamento', query);
    }
};

export default faturamentoService;
