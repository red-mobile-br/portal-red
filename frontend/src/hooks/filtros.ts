import format from "date-fns/format";
import { ptBR } from "date-fns/locale";
import parse from "date-fns/parse";
import startOfMonth from "date-fns/startOfMonth";
import { computed, reactive } from "vue";
import { useRoute, useRouter } from "vue-router";
import $router from '../router';
import { endOfMonth } from "date-fns";

export interface EstadoFiltrosPadrao {
    pagina: number;
    totalPaginas: number;
    tamanho: number;
    ordenarPor: string;
    direcao: "desc" | "asc",
    status: string;
    modalDataAberto: boolean;
    datas: { start: Date, end: Date },
    idRepresentante: string;
    nomeRepresentante: string;
    idGerente: string;
    nomeGerente: string;
    idCliente: string;
    nomeCliente: string;
}

export function useFiltrosPadrao(data?: { defaultStatus: string }) {

    const { query, name } = useRoute();
    const router = useRouter();

    const pagina = query['pagina'] ? +query['pagina'] : 1;
    const totalPaginas = query['totalPaginas'] ? +query['totalPaginas'] : 1;
    const tamanho = query['tamanho'] ? +query['tamanho'] : 12;
    const ordenarPor = query['ordenarPor'] ? query['ordenarPor'].toString() : "";
    const direcao = query['direcao'] ? query['direcao'].toString() as "desc" | "asc" : "desc";
    const status = query['status'] ? query['status'].toString() : data?.defaultStatus || "";
    const start = query['start'] ? parse(query['start'].toString(), "yyyy-MM-dd", new Date()) : startOfMonth(new Date());
    const end = query['end'] ? parse(query['end'].toString(), "yyyy-MM-dd", new Date()) : new Date();
    const idRepresentante = query['idRepresentante'] ? query['idRepresentante'].toString(): "";
    const nomeRepresentante = query['nomeRepresentante'] ? query['nomeRepresentante'].toString(): "";
    const idGerente = query['idGerente'] ? query['idGerente'].toString(): "";
    const nomeGerente = query['nomeGerente'] ? query['nomeGerente'].toString(): "";
    const idCliente = query['idCliente'] ? query['idCliente']?.toString(): "";
    const nomeCliente = query['nomeCliente'] ? query['nomeCliente'].toString(): "";

    const filtros = reactive<EstadoFiltrosPadrao>({
        pagina: pagina,
        totalPaginas: totalPaginas,
        tamanho: tamanho,
        ordenarPor: ordenarPor,
        direcao: direcao,
        status: status,
        modalDataAberto: false,
        datas: { start, end },
        idRepresentante: idRepresentante,
        nomeRepresentante: nomeRepresentante,
        idGerente: idGerente,
        nomeGerente: nomeGerente,
        idCliente: idCliente,
        nomeCliente: nomeCliente
    });

    const filtroMes = computed({
        get: () => filtros.datas.start,
        set: (value: Date) => {
            filtros.datas.start = startOfMonth(value);
            filtros.datas.end = endOfMonth(value);
        }
    });

    const periodo = computed(() => {
        const inicioPeriodo = format(filtros.datas.start as Date, "dd/MMM 'de' yyyy", { locale: ptBR });
        const fimPeriodo = format(filtros.datas.end as Date, "dd/MMM 'de' yyyy", { locale: ptBR });
        return inicioPeriodo == fimPeriodo
            ? inicioPeriodo
            : `${inicioPeriodo} à ${fimPeriodo}`;
    });

    /** Persistir dados */
    const persistirDados = (data: Record<string, string | number>) => {
        sessionStorage.setItem(name!.toString(), JSON.stringify(data));
        router.replace({ query: data });
    };

    /** Aplicar filtro de status */
    const aplicarFiltro = (filtro: keyof EstadoFiltrosPadrao) => {
        const novaQuery = { ...$router.currentRoute.value.query } as Record<string, string | number>;
        filtros[filtro] ? novaQuery[filtro] = filtros.status : delete novaQuery[filtro];
        persistirDados(novaQuery);
    };

    /** Aplicar filtros */
    const aplicarFiltros = () => {
        const novaQuery = { ...$router.currentRoute.value.query } as Record<string, string | number>;

        const aplicar = (key: string, value: string | number) => {
            novaQuery[key] = value;
        };

        filtros.pagina > 1 ? aplicar('pagina', filtros.pagina) : delete novaQuery['pagina'];
        filtros.ordenarPor ? aplicar('ordenarPor', filtros.ordenarPor) : delete novaQuery['ordenarPor'];
        filtros.direcao == 'asc' ? aplicar('direcao', filtros.direcao) : delete novaQuery['direcao'];
        filtros.status ? novaQuery['status'] = filtros.status : delete novaQuery['status'];
        filtros.idRepresentante ? novaQuery['idRepresentante'] = filtros.idRepresentante : delete novaQuery['idRepresentante'];
        filtros.nomeRepresentante ? novaQuery['nomeRepresentante'] = filtros.nomeRepresentante : delete novaQuery['nomeRepresentante'];
        filtros.idGerente ? novaQuery['idGerente'] = filtros.idGerente : delete novaQuery['idGerente'];
        filtros.nomeGerente ? novaQuery['nomeGerente'] = filtros.nomeGerente : delete novaQuery['nomeGerente'];
        filtros.idCliente ? novaQuery['idCliente'] = filtros.idCliente : delete novaQuery['idCliente'];
        filtros.nomeCliente ? novaQuery['nomeCliente'] = filtros.nomeCliente : delete novaQuery['nomeCliente'];

        novaQuery['start'] = format(filtros.datas.start as Date, "yyyy-MM-dd");
        novaQuery['end'] = format(filtros.datas.end as Date, "yyyy-MM-dd");

        persistirDados(novaQuery);
    };

    /** Aplicar filtros e recarregar dados */
    const aplicarFiltrosEReiniciar = (cb: () => void) => {
        filtros.pagina = 1;
        aplicarFiltros();
        cb();
    };

    /** Limpar filtro de status */
    const limparFiltroStatus = (cb: () => void) => {
        filtros.status = '';
        aplicarFiltrosEReiniciar(cb);
    };

    /** Limpar filtro de representante */
    const limparFiltroRepresentante = (cb: () => void) => {
        filtros.idRepresentante = '';
        filtros.nomeRepresentante = '';
        aplicarFiltrosEReiniciar(cb);
    };

    /** Limpar filtro de gerente */
    const limparFiltroGerente = (cb: () => void) => {
        filtros.idGerente = '';
        filtros.nomeGerente = '';
        aplicarFiltrosEReiniciar(cb);
    };

    /** Limpar filtro de cliente */
    const limparFiltroCliente = (cb: () => void) => {
        filtros.idCliente = '';
        filtros.nomeCliente = '';
        aplicarFiltrosEReiniciar(cb);
    };

    return { filtros, aplicarFiltros, aplicarFiltro, periodo, aplicarFiltrosEReiniciar, limparFiltroStatus, limparFiltroRepresentante, limparFiltroGerente, limparFiltroCliente, filtroMes };
}
