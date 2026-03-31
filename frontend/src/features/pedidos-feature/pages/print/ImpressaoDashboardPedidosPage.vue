<script lang="ts" setup>
import { computed, onMounted, reactive } from 'vue';
import format from 'date-fns/format';

import {
    RmPrintView,
    RmPrintPage,
    RmText,
    RmDivider,
    RmTextField,
    RmLoading,
    RmBarChart,
    RmDonutChart
} from '@/components';
import { mascaraCnpj, formatarData } from '@/utils/string-functions';
import { formatarDecimal } from '@/utils/number-functions';
import statusPedidoEnumParser from '@/core/enums/status-pedido-enum-parser';

import pedidoService from '@/services/pedido-service';
import PedidoListaItemDTO from '@/core/dtos/pedido/PedidoListaItemDTO';
import DashboardPedidosDTO from '@/core/dtos/pedido/DashboardPedidosDTO';
import { useFiltrosPadrao } from '@/hooks/filtros';
import useAppStore from '@/store/app-store';
import { useRoute } from "vue-router";
import orcamentoService from "@/services/orcamento-service";

interface EstadoImpressaoPedidos {
    totalPedidos: number;
    pedidosEmAberto: number;
    carregando: boolean;
    pedidos: PedidoListaItemDTO[]
    dashboard: DashboardPedidosDTO;
}

const { filtros } = useFiltrosPadrao();
const { nomeUsuario: nome } = useAppStore();
const route = useRoute();

const state = reactive<EstadoImpressaoPedidos>({
    totalPedidos: 0,
    pedidosEmAberto: 0,
    carregando: true,
    pedidos: [],
    dashboard: {
        totalPedidosPeriodo: 0,
        pedidosAbertosPeriodo: 0,
        valorTotalPedidos: 0,
        pedidosPorPeriodo: [],
        pedidosPorTipo: []
    }
});

const nomeUsuario = computed(() => filtros.nomeRepresentante || nome.value);
const intervaloData = computed(() => `${format(filtros.datas.start as Date, 'dd/MM/yyyy')} à ${format(filtros.datas.end as Date, 'dd/MM/yyyy')}`);

const ehPaginaOrcamento = computed(() => route.name!.toString().indexOf('orcamento') >= 0);
const titulo = ehPaginaOrcamento.value ? "Orçamentos" : "Pedidos";

const carregarDados = async () => {
    try {
        state.carregando = true;

        const consultaPedidos = {
            de: filtros.datas.start as Date,
            ate: filtros.datas.end as Date,
            pagina: filtros.pagina,
            tamanho: 9999,
            status: filtros.status,
            ordenarPor: filtros.ordenarPor,
            direcao: filtros.direcao,
            idRepresentante: filtros.idRepresentante
        };

        const consultaDashboard = {
            de: filtros.datas.start as Date,
            ate: filtros.datas.end as Date,
            status: filtros.status,
            ordenarPor: filtros.ordenarPor,
            direcao: filtros.direcao,
            idRepresentante: filtros.idRepresentante
        };

        const [requisicaoPedidos] = ehPaginaOrcamento.value ? orcamentoService.obterLista(consultaPedidos) : pedidoService.obterLista(consultaPedidos);
        const [requisicaoDashboard] = ehPaginaOrcamento.value ? orcamentoService.obterDashboard(consultaDashboard) : pedidoService.obterDashboard(consultaDashboard);
        const [respostaPedidos, respostaDashboard] = await Promise.all([requisicaoPedidos, requisicaoDashboard]);

        state.pedidos = respostaPedidos.dados;
        state.dashboard = respostaDashboard;
    } catch (error) {
        window.alert("Não foi possível carregar os dados. Por favor, tente novamente.");
    }
    finally {
        state.carregando = false;
    }
};

onMounted(() => carregarDados());
</script>

<template>
    <RmPrintView>
        <transition name="fade" mode="out-in">
            <div v-if="state.carregando" class="h-screen flex items-center justify-center">
                <RmLoading />
            </div>

            <RmPrintPage v-else title="Relatório de pedidos">
                <div class="w-full">

                    <!-- Header -->
                    <div class="flex items-stretch">
                        <div>
                            <RmText type="headline-small" class="flex-1">
                                Período
                            </RmText>
                            <RmText type="display-large">
                                {{ intervaloData }}
                            </RmText>
                        </div>
                        <div class="mx-5 w-px bg-gray-300" />
                        <div>
                            <RmText type="headline-small" class="flex-1">
                                Total de {{ titulo.toLowerCase() }}:
                            </RmText>
                            <RmText type="display-large">
                                {{ state.dashboard.totalPedidosPeriodo }}
                            </RmText>
                        </div>
                        <div class="mx-5 w-px bg-gray-300" />
                        <div>
                            <RmText type="headline-small" class="flex-1">
                                {{ titulo }} em aberto
                            </RmText>
                            <RmText type="display-large">
                                {{ state.dashboard.pedidosAbertosPeriodo }}
                            </RmText>
                        </div>
                    </div>

                    <RmDivider class="my-5" />
                    <RmText type="headline-small" class="mb-4">
                        Representante
                    </RmText>
                    <RmTextField>{{ nomeUsuario }}</RmTextField>
                    <RmDivider class="my-5" />

                    <div class="flex items-stretch">
                        <div class="w-1/2">
                            <RmText type="headline-small" class="mb-4">
                                {{ titulo }} durante o período
                            </RmText>
                            <div class="h-44 relative">
                                <RmBarChart :items="state.dashboard.pedidosPorPeriodo" />
                            </div>
                        </div>

                        <div class="mx-5 w-px bg-gray-300" />

                        <div class="w-1/2">
                            <RmText type="headline-small" class="mb-4">
                                {{ titulo }} por status
                            </RmText>
                            <div class="h-44 relative">
                                <RmDonutChart :items="state.dashboard.pedidosPorTipo" />
                            </div>
                        </div>
                    </div>

                    <RmDivider class="my-5" />

                    <!-- Produtos -->
                    <RmText type="headline-small" class="mb-4">
                        {{ titulo }}
                    </RmText>

                    <table class="red-mobile-table --ultra-small">
                        <tr>
                            <th class="w-12">
                                {{ ehPaginaOrcamento ? "Orçamento" : "Pedido" }}
                            </th>
                            <th class="w-16">
                                Cód. cliente
                            </th>
                            <th class="w-8">
                                Loja
                            </th>
                            <th class="text-left">
                                Cliente
                            </th>
                            <th class="w-28">
                                CNPJ
                            </th>
                            <th class="w-16">
                                Emissão
                            </th>
                            <th class="w-20">
                                Valor total
                            </th>
                            <th class="w-32">
                                Status
                            </th>
                        </tr>
                        <tr v-for="pedido in state.pedidos" :key="pedido.id">
                            <td>{{ pedido.id }}</td>
                            <td>{{ pedido.idCliente.substr(0,6) }}</td>
                            <td>{{ pedido.idCliente.substr(6,2) }}</td>
                            <td class="!text-left">
                                {{ pedido.nome }}
                            </td>
                            <td>{{ mascaraCnpj(pedido.cnpj) }}</td>
                            <td>{{ formatarData(pedido.dataLancamento) }}</td>
                            <td>R$ {{ formatarDecimal(pedido.valorTotal) }}</td>
                            <td>{{ statusPedidoEnumParser.get(pedido.status)?.titulo }}</td>
                        </tr>
                        <tr v-if="state.pedidos.length == 0">
                            <td colspan="8">
                                Nenhum {{ ehPaginaOrcamento ? "orçamento" : "pedido" }} durante o período
                            </td>
                        </tr>
                    </table>

                </div>
            </RmPrintPage>
        </transition>
    </RmPrintView>
</template>
