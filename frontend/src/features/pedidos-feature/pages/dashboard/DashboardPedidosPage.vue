<script lang="ts" setup>
import orcamentoService from "@/services/orcamento-service";
import { computed, onMounted, reactive, ref } from 'vue';

// Libs
import { Canceler } from 'axios';
import { useRoute, useRouter } from 'vue-router';
import format from 'date-fns/format';

// Componentes
import {
    RmText,
    RmCard,
    RmFilterTag,
    RmDateFilterModal,
    RmDivider,
    RmDonutChart,
    RmPaginator,
    RmLoading,
    RmFilterTagButton,
    RmBuscarRepresentanteModal,
    RmBuscarGerenteModal,
    BuscarRepresentanteModalInstancia,
    BuscarGerenteModalInstancia,
    RmBarChart
} from '@/components';

// Includes
import DialogoAcompanharPedido from '@/features/pedidos-feature/pages/dashboard/components/DialogoAcompanharPedido.vue';
import ListaPedidos from '@/features/pedidos-feature/pages/dashboard/components/ListaPedidos.vue';

// Hooks
import { useFiltrosPadrao } from '@/hooks/filtros';
import { useDownloader } from '@/hooks/downloader';
import { useAutorizacao } from '@/hooks/autorizacao';
import { useToast } from '@/hooks/toast';
import { useAlert } from '@/hooks/alerta';

// Services e DTOS
import DashboardPedidosDTO from '@/core/dtos/pedido/DashboardPedidosDTO';
import PedidoListaItemDTO from '@/core/dtos/pedido/PedidoListaItemDTO';
import pedidoService from '@/services/pedido-service';
import ErroDTO from '@/core/dtos/ErroDTO';

import statusPedidoEnum from '@/core/enums/status-pedido-enum-parser';
import { PrintIcon } from '@/icons';
import { formatarDecimal } from '@/utils/number-functions';

interface EstadoGerenciarPedidos {
    pedidoSelecionado: PedidoListaItemDTO | null;
    pedidos: PedidoListaItemDTO[];
    carregandoDashboard: boolean;
    carregandoLista: boolean;
    dashboard: DashboardPedidosDTO
}

const { filtros, aplicarFiltros, periodo, aplicarFiltrosEReiniciar, limparFiltroStatus, limparFiltroRepresentante, limparFiltroGerente } = useFiltrosPadrao();
const { eGerente, eAdmin } = useAutorizacao();

const { baixarBase64 } = useDownloader();
const router = useRouter();
const route = useRoute();

const toast = useToast();
const alert = useAlert();

/** Estado */
const state = reactive<EstadoGerenciarPedidos>({
    pedidoSelecionado: null,
    pedidos: [],
    carregandoDashboard: true,
    carregandoLista: false,
    dashboard: {
        totalPedidosPeriodo: 0,
        pedidosAbertosPeriodo: 0,
        valorTotalPedidos: 0,
        pedidosPorPeriodo: [],
        pedidosPorTipo: []
    }
});

// Refs
const refModalBuscarRepresentante = ref<BuscarRepresentanteModalInstancia | null>(null);
const refModalBuscarGerente = ref<BuscarGerenteModalInstancia | null>(null);

// Filtros
const opcoesStatus = Array.from(statusPedidoEnum.entries()).map<{ name: string, value: string }>(([chave, el]) => {
    return {
        name: el.titulo,
        value: chave
    };
});

const ehPaginaOrcamento = computed(() => route.name!.toString().indexOf('orcamento') >= 0);
const titulo = ehPaginaOrcamento.value ? "Orçamentos" : "Pedidos";

// ======== Métodos =========

/** Carregar pedidos no período */
let canceladorPedidos: Canceler | null;
async function obterPedidos(pagina: number) {

    try {
        canceladorPedidos && canceladorPedidos();
        state.carregandoLista = true;
        aplicarFiltros();

        const query = {
            de: filtros.datas.start,
            ate: filtros.datas.end,
            pagina: pagina,
            tamanho: filtros.tamanho,
            ordenarPor: filtros.ordenarPor,
            direcao: filtros.direcao,
            status: filtros.status,
            idRepresentante: filtros.idRepresentante,
            idGerente: filtros.idGerente
        };

        const [requisicao, cancelador] = ehPaginaOrcamento.value
            ? orcamentoService.obterLista(query)
            : pedidoService.obterLista(query);

        canceladorPedidos = cancelador;
        const resp = await requisicao;

        state.pedidos = resp.dados;
        filtros.totalPaginas = Math.max(resp.totalPaginas, 1);
        state.carregandoLista = false;

    } catch (e) {
        const error = e as ErroDTO;
        alert({ titulo: 'Erro', mensagem: error.mensagem || 'Não foi possível carregar os pedidos.' });
    }
    finally {
        state.carregandoLista = false;
    }
}

/** Carregar dados da tela */
const carregarDados = async () => {
    try {
        state.carregandoDashboard = true;

        const queryPedidos = {
            de: filtros.datas.start,
            ate: filtros.datas.end,
            pagina: filtros.pagina,
            tamanho: filtros.tamanho,
            status: filtros.status,
            ordenarPor: filtros.ordenarPor,
            direcao: filtros.direcao,
            idRepresentante: filtros.idRepresentante,
            idGerente: filtros.idGerente
        };

        const queryDashboard = {
            de: filtros.datas.start,
            ate: filtros.datas.end,
            status: filtros.status,
            ordenarPor: filtros.ordenarPor,
            direcao: filtros.direcao,
            idRepresentante: filtros.idRepresentante,
            idGerente: filtros.idGerente
        };

        const [requisicaoPedidos] = ehPaginaOrcamento.value
            ? orcamentoService.obterLista(queryPedidos)
            : pedidoService.obterLista(queryPedidos);

        const [requisicaoDashboard] = ehPaginaOrcamento.value
            ? orcamentoService.obterDashboard(queryDashboard)
            : pedidoService.obterDashboard(queryDashboard);

        const [respostaPedidos, respostaDashboard] = await Promise.all([requisicaoPedidos, requisicaoDashboard]);

        state.pedidos = respostaPedidos.dados;
        state.dashboard = respostaDashboard;
        filtros.totalPaginas = Math.max(respostaPedidos.totalPaginas, 1);
    } catch (error) {
        alert({ mensagem: "Não foi possível carregar os dados, por favor, tente novamente" });
    }
    finally {
        state.carregandoDashboard = false;
    }
};

/** Baixar nota fiscal */
const baixarNotaFiscal = async (dados: { idPedido: string; notaFiscal: string }) => {
    const { idPedido, notaFiscal } = dados;

    toast({ mensagem: 'Aguarde enquanto processamos a nota fiscal' });
    try {
        const [requisicao] = pedidoService.obterNotaFiscal(idPedido, notaFiscal);
        const resp = await requisicao;
        await baixarBase64({ base64: resp.data, nomeArquivo: `nf-${notaFiscal}.pdf`, tipoMime: 'application/pdf' });
    } catch (error) {
        toast({ mensagem: "Não foi possível baixar a NF. Por favor, tente novamente" });
    }
};

/** Baixar boleto */
const baixarBoleto = async (dados: { idPedido: string; boleto: string }) => {
    const { idPedido, boleto } = dados;
    toast({ mensagem: 'Aguarde enquanto processamos o seu boleto' });
    try {
        const [requisicao] = pedidoService.obterBoleto(idPedido, boleto);
        const resp = await requisicao;
        await baixarBase64({ base64: resp.data, nomeArquivo: `boleto-${boleto}.pdf`, tipoMime: 'application/pdf' });
    } catch (error) {
        toast({ mensagem: "Não foi possível baixar o boleto. Por favor, tente novamente" });
    }
};

/** Imprimir */
const imprimir = () => {
    const rota = router.resolve({
        name: ehPaginaOrcamento.value ? 'impressaoOrcamentos' : 'impressaoPedidos',
        query: {
            start: format(filtros.datas.start, "yyyy-MM-dd"),
            end: format(filtros.datas.end, "yyyy-MM-dd"),
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
        filtros.idRepresentante = representante.id;
        filtros.nomeRepresentante = representante.nome;
        aplicarFiltrosEReiniciar(carregarDados);
    }
};

/** Abrir modal de busca de gerentes */
const buscarGerente = async () => {
    const gerente = await refModalBuscarGerente.value?.search();
    if(gerente) {
        filtros.idGerente = gerente.id;
        filtros.nomeGerente = gerente.nome;
        aplicarFiltrosEReiniciar(carregarDados);
    }
};

onMounted(() => carregarDados());
</script>

<template>
    <Transition name="fade" mode="out-in">
        <div v-if="state.carregandoDashboard" key="1" class="h-novo-pedido-card flex items-center justify-center">
            <RmLoading />
        </div>

        <div v-else key="2" class="grid flex-1 gap-4 pedidos-dashboard pb-4">
            <!-- Filtros -->
            <div class="md:col-span-3 xl:col-span-3 flex flex-wrap -mb-2">

                <!-- Período -->
                <RmFilterTag @click="filtros.modalDataAberto = true">
                    {{ periodo }}
                </RmFilterTag>

                <!-- Status -->
                <RmFilterTagButton v-model="filtros.status"
                                   :options="opcoesStatus"
                                   :show-clear-button="!!filtros.status"
                                   placeholder="Status"
                                   @on:change="() => aplicarFiltrosEReiniciar(carregarDados)"
                                   @on:clear="limparFiltroStatus(carregarDados)" />

                <!-- Gerente -->
                <RmFilterTag v-if="eAdmin"
                             :show-clear-button="!!filtros.idGerente"
                             @click="buscarGerente"
                             @on:clear="limparFiltroGerente(carregarDados)">
                    {{ filtros.nomeGerente || "Gerente" }}
                </RmFilterTag>

                <!-- Representante -->
                <RmFilterTag v-if="eGerente"
                             :show-clear-button="!!filtros.idRepresentante"
                             @click="buscarRepresentante"
                             @on:clear="limparFiltroRepresentante(carregarDados)">
                    {{ filtros.nomeRepresentante || "Representante" }}
                </RmFilterTag>

                <!-- Imprimir -->
                <RmFilterTag :hide-arrow="true" @click="imprimir">
                    <PrintIcon class="w-4 fill-primary" />
                </RmFilterTag>
            </div>

            <!-- Total de pedidos -->
            <RmCard class="!py-3">
                <RmText type="headline-small" class="mb-1">
                    Total de {{ titulo.toLowerCase() }}
                </RmText>

                <p class="font-semibold text-2xl text-primary-light mb-2.5">
                    {{ state.dashboard.totalPedidosPeriodo }}
                </p>

                <RmDivider class="mb-1" />

                <RmText type="label-small">
                    Durante o período
                </RmText>
            </RmCard>

            <!-- Pedidos em aberto -->
            <RmCard class="!pb-3 xl:row-start-3 xl:col-start-1">
                <RmText type="headline-small" class="mb-1">
                    {{ titulo }} em aberto
                </RmText>
                <p class="font-semibold text-2xl text-accent mb-2.5">
                    {{ state.dashboard.pedidosAbertosPeriodo }}
                </p>
                <RmDivider class="mb-1" />
                <RmText type="label-small">
                    Durante o período
                </RmText>
            </RmCard>

            <!-- Valor total pedidos -->
            <RmCard class="!pb-3 xl:row-start-4 xl:col-start-1">
                <RmText type="headline-small" class="mb-1">
                    Valor total de {{ titulo.toLowerCase() }}
                </RmText>
                <p class="font-semibold text-2xl text-accent mb-2.5">
                    R$ {{ formatarDecimal(state.dashboard.valorTotalPedidos) }}
                </p>
                <RmDivider class="mb-1" />
                <RmText type="label-small">
                    Durante o período
                </RmText>
            </RmCard>

            <!-- Pedidos durante o período -->
            <RmCard class="md:col-span-2 xl:col-span-1 xl:row-span-3 flex flex-col items-stretch">
                <RmText type="headline-small" class="mb-4">
                    {{ titulo }} durante o período
                </RmText>
                <div class="flex-1 relative">
                    <RmBarChart :items="state.dashboard.pedidosPorPeriodo" />
                </div>
            </RmCard>

            <RmCard class="xl:row-span-3  flex flex-col items-stretch">
                <RmText type="headline-small" class="mb-4">
                    {{ titulo }} por status
                </RmText>
                <div class="flex-1 relative">
                    <RmDonutChart :items="state.dashboard.pedidosPorTipo" />
                </div>
            </RmCard>

            <RmCard class="md:col-span-3 xl:col-span-3 flex flex-col min-h-[19rem]">
                <!-- Últimos pedido -->
                <div class="flex items-center mb-4">
                    <RmText type="headline-small" class="flex-1">
                        Últimos {{ titulo.toLowerCase() }}
                    </RmText>

                    <RmPaginator v-model:page="filtros.pagina"
                                 :total-pages="filtros.totalPaginas"
                                 @on:page-change="obterPedidos" />
                </div>

                <div class="overflow-x-auto light-scroll w-full flex-1">
                    <ListaPedidos v-model:order-by="filtros.ordenarPor"
                                v-model:direction="filtros.direcao"
                                style="min-width: 900px"
                                :loading="state.carregandoLista || state.carregandoDashboard"
                                :items="state.pedidos"
                                :is-quote-page="ehPaginaOrcamento"
                                @on:select-order="state.pedidoSelecionado = $event"
                                @on:select-invoice="baixarNotaFiscal"
                                @on:select-slip="baixarBoleto"
                                @on:order-changed="obterPedidos" />
                </div>
            </RmCard>

            <RmDateFilterModal v-model:isOpened="filtros.modalDataAberto"
                               v-model:dates="filtros.datas"
                               :can-select-future-dates="true"
                               @on:filter="aplicarFiltrosEReiniciar(carregarDados)" />

            <DialogoAcompanharPedido v-model:order="state.pedidoSelecionado" />

            <!-- Modal de buscar representante -->
            <RmBuscarRepresentanteModal ref="refModalBuscarRepresentante" />

            <!-- Modal de buscar gerente -->
            <RmBuscarGerenteModal ref="refModalBuscarGerente" />

        </div>
    </Transition>
</template>

<style>
.pedidos-dashboard {
    grid-template-columns: repeat(1, minmax(0, 1fr));
    grid-template-rows: auto 110px 110px 110px 300px 300px 1fr;
    grid-auto-rows: 1fr ;
}

@media only screen and (min-width: 768px) {
    .pedidos-dashboard {
        grid-template-columns: minmax(0, 1fr) minmax(0, 1fr) minmax(0, 1fr);
        grid-template-rows: auto 110px 400px;
    }
}

@media only screen and (min-width: 1280px) {
    .pedidos-dashboard {
        grid-template-columns: minmax(0, 280px) minmax(0, 1fr) minmax(0, 350px);
        grid-template-rows: auto 110px 110px 110px;
    }
}
</style>
