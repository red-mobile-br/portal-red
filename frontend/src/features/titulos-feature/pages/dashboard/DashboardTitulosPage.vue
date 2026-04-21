<script lang="ts" setup>
import { onMounted, reactive, ref } from 'vue';
import { format } from 'date-fns';

// Componentes
import {
    RmCard,
    RmFilterTag,
    RmText,
    RmDivider,
    RmDateFilterModal,
    RmLoading,
    RmPaginator,
    RmFilterTagButton,
    RmBuscarRepresentanteModal,
    RmBuscarClienteModal,
    BuscarRepresentanteModalInstancia,
    BuscarClienteModalInstancia,
} from '@/components';

// Includes
import ListaTitulos from './components/ListaTitulos.vue';

// Services e DTOS
import TituloListaItemDTO from '@/core/dtos/titulo/TituloListaItemDTO';
import DashboardTitulosDTO from '@/core/dtos/titulo/DashboardTitulosDTO';
import { tituloService } from '@/services/titulo-service';

// Functions / hooks
import { useFiltrosPadrao } from '@/hooks/filtros';
import { Canceler } from 'axios';
import { formatarDecimal } from '@/utils/number-functions';
import { useRouter } from 'vue-router';
import statusTituloEnum from '@/core/enums/status-titulo-enum-parser';
import { useAutorizacao } from '@/hooks/autorizacao';
import { PrintIcon } from '@/icons';
import GraficoDesempenho from './components/GraficoDesempenho.vue';

interface EstadoDashboardTitulos {
    carregandoDashboard: boolean;
    carregandoLista: boolean;
    titulos: TituloListaItemDTO[],
    dashboard: DashboardTitulosDTO
}

const {
    filtros,
    periodo,
    aplicarFiltros,
    aplicarFiltrosEReiniciar,
    limparFiltroStatus,
    limparFiltroRepresentante,
    limparFiltroCliente
} = useFiltrosPadrao();
const { eGerente } = useAutorizacao();

const router = useRouter();

/** Estado */
const state = reactive<EstadoDashboardTitulos>({
    carregandoDashboard: true,
    carregandoLista: false,
    titulos: [],
    dashboard: {
        valorRecebido: 0,
        valorAReceber: 0,
        desempenhoPorPeriodo: []
    }
});

// Refs
const refModalBuscarRepresentante = ref<BuscarRepresentanteModalInstancia | null>(null);
const refModalBuscarCliente = ref<BuscarClienteModalInstancia | null>(null);

const opcoesStatus = Array.from(statusTituloEnum.entries()).map<{ name: string, value: string }>(([chave, el]) => {
    return {
        name: el.titulo,
        value: chave
    };
});

// ======== Métodos =========
let canceladorTitulos: Canceler | null;

const carregarTitulos = (pagina: number) => {
    canceladorTitulos && canceladorTitulos();
    state.carregandoLista = true;
    aplicarFiltros();

    const [requisicao, cancelador] = tituloService.obterTitulos({
        de: filtros.datas.start as Date,
        ate: filtros.datas.end as Date,
        pagina: pagina,
        tamanho: 12,
        status: filtros.status,
        ordenarPor: filtros.ordenarPor,
        direcao: filtros.direcao,
        idRepresentante: filtros.idRepresentante,
        idCliente: filtros.idCliente
    });

    canceladorTitulos = cancelador;
    requisicao
        .then(resp => {
            state.titulos = resp.dados;
            filtros.totalPaginas = resp.totalPaginas || 1;
            state.carregandoLista = false;
        });
};

/** Carregar dados da tela */
const carregarDados = async () => {
    try {
        state.carregandoDashboard = true;

        const [requisicaoTitulos] = tituloService.obterTitulos({
            de: filtros.datas.start as Date,
            ate: filtros.datas.end as Date,
            pagina: filtros.pagina,
            tamanho: 12,
            status: filtros.status,
            ordenarPor: filtros.ordenarPor,
            direcao: filtros.direcao,
            idRepresentante: filtros.idRepresentante,
            idCliente: filtros.idCliente
        });

        const [requisicaoDashboard] = tituloService.obterDashboardTitulos({
            de: filtros.datas.start as Date,
            ate: filtros.datas.end as Date,
            status: filtros.status,
            ordenarPor: filtros.ordenarPor,
            direcao: filtros.direcao,
            idRepresentante: filtros.idRepresentante,
            idCliente: filtros.idCliente
        });
        const [respostaTitulos, respostaDashboard] = await Promise.all([requisicaoTitulos, requisicaoDashboard]);

        state.titulos = respostaTitulos.dados;
        state.dashboard = respostaDashboard;
        filtros.totalPaginas = respostaTitulos.totalPaginas || 1;

    } catch (error) {
        window.alert("Não foi possível carregar os dados, por favor, tente novamente");
    }
    finally {
        state.carregandoDashboard = false;
    }
};

const imprimir = () => {
    const rota = router.resolve({
        name: 'impressaoTitulos',
        query: {
            from: format(filtros.datas.start as Date, 'dd-MM-yyyy'),
            to: format(filtros.datas.end as Date, 'dd-MM-yyyy'),
            idRepresentante: filtros.idRepresentante,
            nomeRepresentante: filtros.nomeRepresentante,
            idCliente: filtros.idCliente,
            nomeCliente: filtros.nomeCliente,
            status: filtros.status,
        }
    });

    window.open(rota.href, '_blank');
};

/** Abrir modal de busca de representantes */
const buscarRepresentante = async () => {
    const representante = await refModalBuscarRepresentante.value?.search();
    if(representante) {
        filtros.idRepresentante = representante.id ?? '';
        filtros.nomeRepresentante = representante.nome ?? '';
        aplicarFiltrosEReiniciar(carregarDados);
    }
};

/** Abrir modal de busca de clientes */
const buscarCliente = async () => {
    const cliente = await refModalBuscarCliente.value?.search();
    if(cliente) {
        filtros.idCliente = cliente.id ?? '';
        filtros.nomeCliente = cliente.nome ?? '';
        aplicarFiltrosEReiniciar(carregarDados);
    }
};

onMounted(() => carregarDados());
</script>

<template>
    <transition name="fade" mode="out-in">
        <div v-if="state.carregandoDashboard" key="1" class="h-novo-pedido-card flex items-center justify-center">
            <RmLoading />
        </div>

        <div v-else key="2" class="flex-1 grid gap-4 titulos-dashboard">

            <!-- Filtros -->
            <div class="md:col-span-2 flex flex-wrap -mb-2">
                <RmFilterTag @click="filtros.modalDataAberto = true">
                    {{ periodo }}
                </RmFilterTag>

                <!-- Status -->
                <RmFilterTagButton v-model="filtros.status"
                                   :options="opcoesStatus"
                                   :show-clear-button="filtros.status.length > 0"
                                   placeholder="Status"
                                   @on:change="aplicarFiltrosEReiniciar(carregarDados)"
                                   @on:clear="limparFiltroStatus(carregarDados)" />

                <!-- Representante -->
                <RmFilterTag v-if="eGerente"
                             :show-clear-button="filtros.idRepresentante.length > 0"
                             @click="buscarRepresentante"
                             @on:clear="limparFiltroRepresentante(carregarDados)">
                    {{ filtros.nomeRepresentante || "Representante" }}
                </RmFilterTag>

                <!-- Cliente -->
                <RmFilterTag :show-clear-button="filtros.idCliente.length > 0"
                             @click="buscarCliente"
                             @on:clear="limparFiltroCliente(carregarDados)">
                    {{ filtros.nomeCliente || "Cliente" }}
                </RmFilterTag>

                <RmFilterTag :hide-arrow="true" @click="imprimir">
                    <PrintIcon class="fill-primary w-4" />
                </RmFilterTag>
            </div>

            <!-- Valor Recebido -->
            <RmCard class="!pb-3">
                <RmText type="headline-small" class="mb-2">
                    Valor Recebido
                </RmText>
                <p class="font-semibold text-2xl text-primary-light">
                    R$ {{ formatarDecimal(state.dashboard.valorRecebido ?? 0) }}
                </p>
                <RmDivider class="mb-2 my-3" />
                <RmText type="label-small">
                    Durante o período
                </RmText>
            </RmCard>

            <!-- Valor a Receber -->
            <RmCard class="!pb-3 lg:row-start-3 lg:col-start-1">
                <RmText type="headline-small" class="mb-2">
                    Valor a Receber
                </RmText>
                <p class="font-semibold text-2xl text-accent">
                    R$ {{ formatarDecimal(state.dashboard.valorAReceber ?? 0) }}
                </p>
                <RmDivider class="mb-2 my-3" />
                <RmText type="label-small">
                    Durante o período
                </RmText>
            </RmCard>

            <!-- Desempenho no período -->
            <RmCard class="md:col-span-2 lg:col-span-1 lg:row-span-2 flex flex-col items-stretch !pb-2">
                <RmText type="headline-small" class="mb-2">
                    Desempenho no período
                </RmText>
                <div class="flex-1 relative">
                    <GraficoDesempenho :items="state.dashboard.desempenhoPorPeriodo ?? []" />
                </div>
            </RmCard>

            <RmCard class="md:col-span-2 !rounded-b-none">
                <div class="flex items-center mb-4">
                    <RmText type="headline-small" class="mb-4 flex-1">
                        Últimos títulos
                    </RmText>

                    <RmPaginator v-model:page="filtros.pagina"
                                 :total-pages="filtros.totalPaginas"
                                 @on:page-change="carregarTitulos" />
                </div>
                <div class="overflow-x-auto light-scroll w-full">
                    <ListaTitulos v-model:orderBy="filtros.ordenarPor"
                                v-model:direction="filtros.direcao"
                                style="min-width: 1000px"
                                :items="state.titulos"
                                :loading="state.carregandoLista"
                                @on:order-changed="carregarTitulos" />
                </div>
            </RmCard>


            <RmDateFilterModal v-model:isOpened="filtros.modalDataAberto"
                               v-model:dates="filtros.datas"
                               :can-select-future-dates="true"
                               @on:filter="aplicarFiltrosEReiniciar(carregarDados)" />

            <!-- Modal de buscar representante -->
            <RmBuscarRepresentanteModal ref="refModalBuscarRepresentante" />

            <!-- Modal de buscar cliente -->
            <RmBuscarClienteModal ref="refModalBuscarCliente" />
        </div>
    </transition>
</template>


<style>
.titulos-dashboard{
    grid-template-columns: repeat(1, minmax(0, 1fr));
    grid-template-rows: auto 130px 130px 300px 1fr;
    grid-auto-rows: 1fr ;
}

@media only screen and (min-width: 768px) {
    .titulos-dashboard{
        grid-template-columns: repeat(2, minmax(0, 1fr));
        grid-template-rows: auto 130px 300px;
    }
}

@media only screen and (min-width: 1024px) {
    .titulos-dashboard{
        grid-template-columns: minmax(0, 280px) minmax(0,1fr);
        grid-template-rows: auto 130px 130px;
    }
}
</style>
