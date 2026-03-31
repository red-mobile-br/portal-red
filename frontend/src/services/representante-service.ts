import RespostaPaginadaDTO from "../core/dtos/RespostaPaginadaDTO";
import RepresentanteListaDTO from "../core/dtos/representante/RepresentanteListaDTO";
import FiltroPadraoQuery from "../core/query-objects/FiltroPadraoQuery";
import { obter } from "./base-service";

const representanteService = {
    obterRepresentantes(query: FiltroPadraoQuery) {
        return obter<RespostaPaginadaDTO<RepresentanteListaDTO>>('/representantes', query);
    }
};

export default representanteService;
