<script lang="ts" setup>
import { onMounted, reactive, ref } from 'vue';
import {
    RmCard,
    RmText,
    RmDivider,
    RmFilterTag,
    RmPaginator,
    RmLoading,
    RmFilterTagButton,
    RmBuscarRepresentanteModal,
    BuscarRepresentanteModalInstancia,
    RmBarChart,
    RmAreaChart,
    RmMonthRangePicker
} from '@/components';

import ListaComissoes from './components/ListaComissoes.vue';

import DashboardComissoesDTO from '@/core/dtos/comissoes/DashboardComissoesDTO';
import ComissaoListaItemDTO from '@/core/dtos/comissoes/ComissaoListaItemDTO';
import comissaoService from '@/services/comissao-service';
import { useFiltrosPadrao } from '@/hooks/filtros';
import { Canceler } from 'axios';
import { formatarDecimal, arredondarPreco } from '@/utils/number-functions';

import { useRouter } from 'vue-router';
import { format } from 'date-fns';
import { useAutorizacao } from '@/hooks/autorizacao';
import { PrintIcon } from '@/icons';

interface EstadoDashboardComissoes {
    carregandoDashboard: boolean;
    carregandoLista: boolean;
    dashboard: DashboardComissoesDTO;
    comissoes: ComissaoListaItemDTO[];
}

const { filtros, aplicarFiltros, limparFiltroRepresentante, aplicarFiltrosEReiniciar } = useFiltrosPadrao({ defaultStatus: "1" });
const { eGerente } = useAutorizacao();

const router = useRouter();

// Estado
const state = reactive<EstadoDashboardComissoes>({
    carregandoDashboard: true,
    carregandoLista: false,
    dashboard: {
        totalComissaoPorPeriodo: 0,
        percentualComissaoPorPeriodo: 0,
        comissoesPorPeriodo: [],
        maiorComissaoPorPeriodo: []
    },
    comissoes: [],
});

// Refs
const refModalBuscarRepresentante = ref<BuscarRepresentanteModalInstancia | null>(null);

// Variáveis
const filtroStatusComissoes = [
    { name: "Todos", value: "0" },
    { name: "Previstas", value: "1" },
    { name: "Geradas", value: "2" }
];

// ======== Métodos =========
let canceladorComissoes: Canceler | null;

const obterComissoes = () => {
    canceladorComissoes && canceladorComissoes();
    state.carregandoLista = true;
    aplicarFiltros();

    const [requisicao, cancelador] = comissaoService.obterComissoes({
        de: filtros.datas.start as Date,
        ate: filtros.datas.end as Date,
        pagina: filtros.pagina,
        tamanho: 12,
        status: filtros.status,
        ordenarPor: filtros.ordenarPor,
        direcao: filtros.direcao,
        idRepresentante: filtros.idRepresentante
    });
    canceladorComissoes = cancelador;
    requisicao
        .then(resp => {
            state.comissoes = resp.dados;
            filtros.totalPaginas = resp.totalPaginas || 1;
            state.carregandoLista = false;
        });
};

/** Carregar dados da tela */
const carregarDados = async () => {
    try {

        state.carregandoDashboard = true;
        const [requisicaoComissoes] = comissaoService.obterComissoes({
            de: filtros.datas.start as Date,
            ate: filtros.datas.end as Date,
            pagina: filtros.pagina,
            tamanho: 12,
            status: filtros.status,
            ordenarPor: filtros.ordenarPor,
            direcao: filtros.direcao,
            idRepresentante: filtros.idRepresentante
        });

        const [requisicaoDashboard] = comissaoService.obterDashboardComissoes({
            de: filtros.datas.start as Date,
            ate: filtros.datas.end as Date,
            status: filtros.status,
            ordenarPor: filtros.ordenarPor,
            direcao: filtros.direcao,
            idRepresentante: filtros.idRepresentante
        });

        const [respostaComissoes, respostaDashboard] = await Promise.all([requisicaoComissoes, requisicaoDashboard]);
        state.comissoes = respostaComissoes.dados;
        state.dashboard = respostaDashboard;
        filtros.totalPaginas = respostaComissoes.totalPaginas || 1;

    } catch (error) {
        window.alert("Não foi possível carregar os dados, por favor, tente novamente");
    }
    finally {
        state.carregandoDashboard = false;
    }
};

/** Imprimir */
const imprimir = () => {
    const rota = router.resolve({
        name: 'impressaoComissoes',
        query: {
            from: format(filtros.datas.start as Date, 'dd-MM-yyyy'),
            to: format(filtros.datas.end as Date, 'dd-MM-yyyy'),
            idRepresentante: filtros.idRepresentante,
            nomeRepresentante: filtros.nomeRepresentante,
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


onMounted(() => carregarDados());
</script>

<template>
    <transition name="fade" mode="out-in">

        <div v-if="state.carregandoDashboard" key="1" class="h-novo-pedido-card flex items-center justify-center">
            <RmLoading />
        </div>

        <div v-else key="2" class="flex-1 grid gap-4 comissoes-dashboard">

            <!-- Filtros -->
            <div class="flex flex-wrap md:col-span-2 xl:col-span-3 -mb-2">
                <RmMonthRangePicker v-model="filtros.datas"
                                    @on:date-change="() => aplicarFiltrosEReiniciar(carregarDados)" />

                <RmFilterTagButton v-model="filtros.status"
                                   :options="filtroStatusComissoes"
                                   placeholder="Status"
                                   @on:change="carregarDados" />

                <!-- Representante -->
                <RmFilterTag v-if="eGerente"
                             :show-clear-button="filtros.idRepresentante.length > 0"
                             @click="buscarRepresentante"
                             @on:clear="limparFiltroRepresentante(carregarDados)">
                    {{ filtros.nomeRepresentante || "Representante" }}
                </RmFilterTag>


                <RmFilterTag :hide-arrow="true" @click="imprimir">
                    <PrintIcon class="fill-primary w-4" />
                </RmFilterTag>
            </div>

            <!-- Valor Total de comissão -->
            <RmCard class="!pb-3">
                <RmText type="headline-small" class="mb-2">
                    Valor total de comissão
                </RmText>
                <p class="font-semibold text-2xl text-primary-light">
                    R$ {{ formatarDecimal(state.dashboard.totalComissaoPorPeriodo ?? 0) }}
                </p>
                <RmDivider class="mb-2 my-3" />
                <RmText type="label-small">
                    Durante o período
                </RmText>
            </RmCard>

            <!-- Porcentagem média -->
            <RmCard class="!pb-3 xl:row-start-3">
                <RmText type="headline-small" class="mb-2">
                    Porcentagem média
                </RmText>
                <p class="font-semibold text-2xl text-accent">
                    {{ formatarDecimal(arredondarPreco(state.dashboard.percentualComissaoPorPeriodo ?? 0)) }}%
                </p>
                <RmDivider class="mb-2 my-3" />
                <RmText type="label-small">
                    Durante o período
                </RmText>
            </RmCard>

            <!-- Desempenho no período -->
            <RmCard class="flex flex-col items-stretch !pb-4 md:col-span-2 xl:col-span-1 xl:row-span-2">
                <RmText type="headline-small" class="mb-4">
                    Desempenho no período
                </RmText>
                <div class="flex-1 transition-opacity duration-300 relative">
                    <RmAreaChart :items="state.dashboard.comissoesPorPeriodo ?? []" />
                </div>
            </RmCard>

            <!-- Maiores comissões no período -->
            <RmCard class="flex flex-col items-stretch !pb-3 md:col-span-2 xl:col-span-1 xl:row-span-2">
                <RmText type="headline-small" class="mb-4">
                    Maiores comissões no período
                </RmText>
                <div class="flex-1 transition-opacity duration-300 relative">
                    <RmBarChart :items="state.dashboard.maiorComissaoPorPeriodo ?? []" />
                </div>
            </RmCard>

            <!-- Últimas comissões -->
            <RmCard class="md:col-span-2 xl:col-span-3 lg:rounded-b-none">
                <div class="flex items-center mb-4">
                    <RmText type="headline-small" class="flex-1">
                        Últimas comissões
                    </RmText>

                    <RmPaginator v-model:page="filtros.pagina"
                                 :total-pages="filtros.totalPaginas"
                                 @on:page-change="obterComissoes" />
                </div>

                <div class="overflow-x-auto light-scroll w-full min-h-[16rem]">
                    <ListaComissoes v-model:orderBy="filtros.ordenarPor"
                                     v-model:direction="filtros.direcao"
                                     :loading="state.carregandoLista"
                                     :items="state.comissoes"
                                     @on:order-changed="obterComissoes" />
                </div>
            </RmCard>

            <!-- Modal de buscar representante -->
            <RmBuscarRepresentanteModal ref="refModalBuscarRepresentante" />
        </div>
    </transition>
</template>

<style>
.comissoes-dashboard {
    grid-template-columns: repeat(1, minmax(0, 1fr));
    grid-template-rows: auto 130px 130px 300px 300px 1fr;
}

@media (min-width: 768px) {
    .comissoes-dashboard {
        grid-template-columns: minmax(0, 1fr) minmax(0, 1fr);
        grid-template-rows: auto 130px 300px 300px 1fr;
    }
}

@media (min-width: 1280px) {
     .comissoes-dashboard {
         grid-template-columns: minmax(0, 280px) minmax(0, 1fr) minmax(0, 1fr);
         grid-template-rows: auto 130px 130px 1fr;
     }
}
</style>
