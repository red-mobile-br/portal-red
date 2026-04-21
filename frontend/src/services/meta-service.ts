import MetaListaItemDTO from "@/core/dtos/metas/MetaListaItemDTO";
import FiltroPadraoQuery from "../core/query-objects/FiltroPadraoQuery";
import { obter } from "./base-service";
import { formatarData } from "@/utils/string-functions";
import format from "date-fns/format";

const metaService = {
    obterMetas: (query: FiltroPadraoQuery) => {
        return obter<MetaListaItemDTO[]>('/meta', query);
    },
    obterHistorico: (query: FiltroPadraoQuery) => {
        return obter<MetaListaItemDTO[]>('/meta/historico', query);
    },
};

export { metaService };
