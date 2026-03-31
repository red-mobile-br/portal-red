import FiltroPadraoQuery from "../core/query-objects/FiltroPadraoQuery";
import DashboardTitulosDTO from '../core/dtos/titulo/DashboardTitulosDTO';
import { obter } from "./base-service";
import TituloListaItemDTO from "../core/dtos/titulo/TituloListaItemDTO";
import RespostaPaginadaDTO from "../core/dtos/RespostaPaginadaDTO";

const tituloService = {
    obterDashboardTitulos: (query: FiltroPadraoQuery) => {
        return obter<DashboardTitulosDTO>('/titulo/dashboard', query);
    },
    obterTitulos: (query: FiltroPadraoQuery) => {
        return obter<RespostaPaginadaDTO<TituloListaItemDTO>>('/titulo', query);
    }
};

export { tituloService };
