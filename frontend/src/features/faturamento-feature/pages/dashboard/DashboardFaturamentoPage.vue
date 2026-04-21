<script lang="ts" setup>
import { onMounted, reactive, ref } from 'vue';
import { formatarDecimal } from '@/utils/number-functions';

// Componentes
import {
    RmCard,
    RmText,
    RmDivider,
    RmFilterTag,
    RmDateFilterModal,
    RmPaginator,
    RmLoading,
    RmBuscarRepresentanteModal,
    RmBuscarGerenteModal,
    RmDonutChart,
    RmMultiChart,
    BuscarRepresentanteModalInstancia,
    BuscarGerenteModalInstancia

} from '@/components';

// Includes
import ItemListaMaisVendidos from './components/ItemListaMaisVendidos.vue';
import ListaFaturamento from './components/ListaFaturamento.vue';

// Services e DTOS
import FaturamentoListaItemDTO from '@/core/dtos/faturamento/FaturamentoListaItemDTO';
import DashboardFaturamentoDTO from '@/core/dtos/faturamento/DashboardFaturamentoDTO';
import faturamentoService from '@/services/faturamento-service';

// Hooks
import { useFiltrosPadrao } from '@/hooks/filtros';
import { Canceler } from 'axios';

import { useRouter } from 'vue-router';
import { format } from 'date-fns';
import { useAutorizacao } from '@/hooks/autorizacao';
import { PrintIcon } from '@/icons';

interface EstadoDashboardFaturamento {
    faturamentos: FaturamentoListaItemDTO[];
    carregandoDashboard: boolean;
    carregandoLista: boolean;
    dashboard: DashboardFaturamentoDTO
}

// Hooks
const { filtros, periodo, aplicarFiltros, aplicarFiltrosEReiniciar, limparFiltroRepresentante, limparFiltroGerente } = useFiltrosPadrao();
const { eGerente, eAdmin } = useAutorizacao();

const router = useRouter();

// Estado
const state = reactive<EstadoDashboardFaturamento>({
    carregandoDashboard: true,
    carregandoLista: false,
    faturamentos: [],
    dashboard: {
        totalNF: 0,
        totalBase: 0,
        quantidadeVendas: 0,
        vendasPorCategoria: [],
        produtosMaisVendidos: [],
        vendasPorDia: []
    }
});
// Refs
const refModalBuscarRepresentante = ref<BuscarRepresentanteModalInstancia | null>(null);
const refModalBuscarGerente = ref<BuscarGerenteModalInstancia | null>(null);

// ======== Métodos =========
let canceladorFaturamentos: Canceler | null;
const carregarFaturamentos = (pagina: number) => {
    canceladorFaturamentos && canceladorFaturamentos();
    state.carregandoLista = true;
    aplicarFiltros();

    const [requisicao, cancelador] = faturamentoService.obterFaturamentos({
        de: filtros.datas.start as Date,
        ate: filtros.datas.end as Date,
        pagina: pagina,
        tamanho: 12,
        ordenarPor: filtros.ordenarPor,
        direcao: filtros.direcao,
        idRepresentante: filtros.idRepresentante,
        idGerente: filtros.idGerente
    });
    canceladorFaturamentos = cancelador;

    requisicao
        .then(resp => {
            state.faturamentos = resp.dados;
            filtros.totalPaginas = resp.totalPaginas;
            state.carregandoLista = false;
        });
};

/** Carregar dados da tela */
const carregarDados = async () => {
    try {
        state.carregandoDashboard = true;

        const [requisicaoFaturamentos] = faturamentoService.obterFaturamentos({
            de: filtros.datas.start as Date,
            ate: filtros.datas.end as Date,
            pagina: filtros.pagina,
            tamanho: 12,
            ordenarPor: filtros.ordenarPor,
            direcao: filtros.direcao,
            idRepresentante: filtros.idRepresentante,
            idGerente: filtros.idGerente
        });

        const [requisicaoDashboard] = faturamentoService.obterDashboardFaturamentos({
            de: filtros.datas.start as Date,
            ate: filtros.datas.end as Date,
            idRepresentante: filtros.idRepresentante,
            idGerente: filtros.idGerente
        });

        const [respostaFaturamentos, respostaDashboard] = await Promise.all([requisicaoFaturamentos, requisicaoDashboard]);

        state.faturamentos = respostaFaturamentos.dados;
        state.dashboard = respostaDashboard;
        filtros.totalPaginas = respostaFaturamentos.totalPaginas;

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
        name: 'impressaoFaturamento',
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

/** Abrir modal de busca de gerentes */
const buscarGerente = async () => {
    const gerente = await refModalBuscarGerente.value?.search();
    if(gerente) {
        filtros.idGerente = gerente.id ?? '';
        filtros.nomeGerente = gerente.nome ?? '';
        aplicarFiltrosEReiniciar(carregarDados);
    }
};

onMounted(() => carregarDados());

</script>

<template>
    <transition name="fade" mode="out-in">
        <div v-if="state.carregandoDashboard" key="1" class="flex-1 flex items-center justify-center">
            <RmLoading />
        </div>

        <div v-else key="2" class="flex-1 grid gap-4 faturamento-dashboard">

            <!-- Filtros -->
            <div class="sm:col-span-3 xl:col-span-4 flex flex-wrap -mb-2">

                <!-- Data -->
                <RmFilterTag @click="filtros.modalDataAberto = true">
                    {{ periodo }}
                </RmFilterTag>

                <!-- Gerente -->
                <RmFilterTag v-if="eAdmin"
                             :show-clear-button="!!filtros.idGerente"
                             @click="buscarGerente"
                             @on:clear="limparFiltroGerente(carregarDados)">
                    {{ filtros.nomeGerente || "Gerente" }}
                </RmFilterTag>

                <!-- Representante -->
                <RmFilterTag v-if="eGerente"
                             :show-clear-button="filtros.idRepresentante.length > 0"
                             @click="buscarRepresentante"
                             @on:clear="limparFiltroRepresentante(carregarDados)">
                    {{ filtros.nomeRepresentante || "Representante" }}
                </RmFilterTag>

                <!-- Imprimir -->
                <RmFilterTag :hide-arrow="true" @click="imprimir">
                    <PrintIcon class="fill-primary w-4" />
                </RmFilterTag>
            </div>

            <!-- valor Total Base -->
            <RmCard class="!pb-3">
                <RmText type="headline-small" class="mb-2">
                    Valor total base
                </RmText>
                <p class="font-semibold text-2xl text-primary-light">
                    R$ {{ formatarDecimal(state.dashboard.totalBase ?? 0) }}
                </p>
                <RmDivider class="mb-2 my-3" />
                <RmText type="label-small">
                    Valor sem substituição tributária
                </RmText>
            </RmCard>


            <!-- Valor TOtal NF -->
            <RmCard class="!pb-3">
                <RmText type="headline-small" class="mb-2">
                    Valor total NF
                </RmText>
                <p class="font-semibold text-2xl text-accent">
                    R$ {{ formatarDecimal(state.dashboard.totalNF ?? 0) }}
                </p>
                <RmDivider class="mb-2 my-3" />
                <RmText type="label-small">
                    Valor com substituição tributária
                </RmText>
            </RmCard>

            <!-- Total de vendas -->
            <RmCard class="!pb-3">
                <RmText type="headline-small" class="mb-2">
                    Vendas
                </RmText>
                <p class="font-semibold text-2xl">
                    {{ state.dashboard.quantidadeVendas }}
                </p>
                <RmDivider class="mb-2 my-3" />
                <RmText type="label-small">
                    Durante o período
                </RmText>
            </RmCard>

            <!-- Vendas por categoria -->
            <RmCard class="sm:col-span-3 xl:col-span-1 xl:row-span-2 !pb-2">
                <RmText type="headline-small" class="mb-2">
                    Vendas por categoria
                </RmText>
                <div class="h-80 relative">
                    <RmDonutChart :items="state.dashboard.vendasPorCategoria ?? []" :display-legend="true" />
                </div>
            </RmCard>

            <!-- Ganhos -->
            <RmCard class="sm:col-span-3 !pb-0">
                <div class="flex items-center mb-2">
                    <RmText type="headline-small" class="flex-1">
                        Vendas
                    </RmText>

                    <div class="w-2 h-2 rounded-full bg-primary-light mr-2" />
                    <p class="text-xs font-semibold  mr-6">
                        Valor total base
                    </p>

                    <div class="w-2 h-2 rounded-full  bg-accent  mr-2" />
                    <p class="text-xs font-semibold">
                        Valor total NF
                    </p>
                </div>
                <div class="h-[10.5rem] relative">
                    <RmMultiChart :types="['line', 'line']" :items="state.dashboard.vendasPorDia ?? []" />
                </div>

            </RmCard>

            <!-- Últimos faturamentos -->
            <RmCard class="sm:col-span-3 lg:rounded-b-none">
                <div class="flex items-center mb-4">
                    <RmText type="headline-small" class="flex-1">
                        Últimos faturamentos
                    </RmText>

                    <RmPaginator v-model:page="filtros.pagina"
                                 :total-pages="filtros.totalPaginas"
                                 @on:page-change="carregarFaturamentos" />
                </div>

                <div class="overflow-x-auto light-scroll">
                    <ListaFaturamento v-model:orderBy="filtros.ordenarPor"
                                  v-model:direction="filtros.direcao"
                                  :loading="state.carregandoLista || state.carregandoDashboard"
                                  :items="state.faturamentos"
                                  @on:order-changed="carregarFaturamentos" />
                </div>
            </RmCard>

            <!-- Produtos mais vendidos -->
            <RmCard class="sm:col-span-3 xl:col-span-1 rounded-b-none">
                <RmText type="headline-small" class="mb-4">
                    Produtos mais vendidos
                </RmText>
                <ItemListaMaisVendidos v-for="(item, index) in state.dashboard.produtosMaisVendidos ?? []"
                                  :key="index"
                                  :item="item" />

            </RmCard>

            <RmDateFilterModal v-model:isOpened="filtros.modalDataAberto"
                               v-model:dates="filtros.datas"
                               @on:filter="aplicarFiltrosEReiniciar(carregarDados)" />

            <!-- Modal de buscar representante -->
            <RmBuscarRepresentanteModal ref="refModalBuscarRepresentante" />

            <!-- Modal de buscar gerente -->
            <RmBuscarGerenteModal ref="refModalBuscarGerente" />
        </div>
    </transition>

</template>

<style>
.faturamento-dashboard {
    grid-template-columns: repeat(1, minmax(0, 1fr));
    grid-template-rows: auto 130px 130px 130px auto auto auto 1fr;
    grid-auto-rows: 1fr ;
}

@media only screen and (min-width: 640px) {
    .faturamento-dashboard {
        grid-template-columns: 1fr 1fr 1fr;
        grid-template-rows: auto 130px auto auto auto 1fr;
    }
}

@media only screen and (min-width: 1280px) {
    .faturamento-dashboard {
        grid-template-columns: 1fr 1fr 1fr 22rem;
        grid-template-rows: auto 130px auto 1fr;
    }
}
</style>
