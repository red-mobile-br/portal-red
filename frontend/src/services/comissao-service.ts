import DashboardComissoesDTO from "../core/dtos/comissoes/DashboardComissoesDTO";
import ComissaoListaItemDTO from "../core/dtos/comissoes/ComissaoListaItemDTO";
import RespostaPaginadaDTO from "../core/dtos/RespostaPaginadaDTO";
import FiltroPadraoQuery from "../core/query-objects/FiltroPadraoQuery";
import { obter } from "./base-service";

const comissaoService = {
    obterDashboardComissoes: (query: FiltroPadraoQuery) => {
        return obter<DashboardComissoesDTO>('/comissao/dashboard', query);
    },
    obterComissoes: (query: FiltroPadraoQuery) => {
        return obter<RespostaPaginadaDTO<ComissaoListaItemDTO>>('/comissao', query);
    }
};

export default comissaoService;
