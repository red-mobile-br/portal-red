<script lang="ts" setup>
import { computed, onMounted, reactive } from 'vue';
import format from 'date-fns/format';

import { formatarDecimal } from '@/utils/number-functions';
import { formatarData } from '@/utils/string-functions';

// Componentes
import {
    RmDonutChart,
    RmMultiChart,
    RmPrintView,
    RmPrintPage,
    RmText,
    RmDivider,
    RmTextField,
    RmLoading
} from '@/components';

import ItemListaMaisVendidos from '../dashboard/components/ItemListaMaisVendidos.vue';

// DTOS
import useAppStore from '@/store/app-store';
import faturamentoService from '@/services/faturamento-service';
import FaturamentoListaItemDTO from '@/core/dtos/faturamento/FaturamentoListaItemDTO';
import DashboardFaturamentoDTO from '@/core/dtos/faturamento/DashboardFaturamentoDTO';
import { useFiltrosPadrao } from '@/hooks/filtros';


interface EstadoImpressaoFaturamento {
    carregando: boolean;
    faturamentos: FaturamentoListaItemDTO[];
    dashboard: DashboardFaturamentoDTO
}

const { filtros } = useFiltrosPadrao();
const { nomeUsuario: nome } = useAppStore();

const state = reactive<EstadoImpressaoFaturamento>({
    carregando: true,
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

const nomeUsuario = computed(() => filtros.nomeRepresentante || nome.value);
const intervaloData = computed(() => `${format(filtros.datas.start as Date, 'dd/MM/yyyy')} à ${format(filtros.datas.end as Date, 'dd/MM/yyyy')}`);

const carregarDados = async () => {
    try {
        state.carregando = true;
        const [requisicaoFaturamentos] = faturamentoService.obterFaturamentos({
            de: filtros.datas.start as Date,
            ate: filtros.datas.end as Date,
            pagina: filtros.pagina,
            tamanho: 9999,
            ordenarPor: filtros.ordenarPor,
            direcao: filtros.direcao,
            idRepresentante: filtros.idRepresentante
        });
        const [requisicaoDashboard] = faturamentoService.obterDashboardFaturamentos({
            de: filtros.datas.start as Date,
            ate: filtros.datas.end as Date,
            idRepresentante: filtros.idRepresentante
        });
        const [respostaFaturamentos, respostaDashboard] = await Promise.all([requisicaoFaturamentos, requisicaoDashboard]);

        state.faturamentos = respostaFaturamentos.dados;
        state.dashboard = respostaDashboard;

    } catch (error) {
        window.alert("Não foi possível carregar os dados, por favor, tente novamente");
    }
    finally {
        state.carregando = false;
    }
};

onMounted(() => carregarDados());

</script>

<template>
    <RmPrintView>
        <Transition name="fade" mode="out-in">
            <div v-if="state.carregando" class="h-screen flex items-center justify-center">
                <RmLoading />
            </div>

            <RmPrintPage v-else title="Relatório de faturamento">
                <div class="w-full">

                    <!-- Header -->
                    <div class="flex items-stretch">
                        <div>
                            <RmText type="headline-small" class="flex-1">
                                Período
                            </RmText>
                            <RmText type="display-medium">
                                {{ intervaloData }}
                            </RmText>
                        </div>
                        <div class="mx-5 w-px bg-gray-300" />
                        <div>
                            <RmText type="headline-small" class="flex-1">
                                Valor total base:
                            </RmText>
                            <RmText type="display-medium">
                                R$ {{ formatarDecimal(state.dashboard.totalBase ?? 0) }}
                            </RmText>
                        </div>
                        <div class="mx-5 w-px bg-gray-300" />
                        <div>
                            <RmText type="headline-small" class="flex-1">
                                Valor total NF
                            </RmText>
                            <RmText type="display-medium">
                                R$ {{ formatarDecimal(state.dashboard.totalNF ?? 0) }}
                            </RmText>
                        </div>
                        <div class="mx-5 w-px bg-gray-300" />
                        <div>
                            <RmText type="headline-small" class="flex-1">
                                Vendas
                            </RmText>
                            <RmText type="display-medium">
                                {{ state.dashboard.quantidadeVendas }}
                            </RmText>
                        </div>
                    </div>

                    <RmDivider class="my-5" />

                    <!-- Representante -->
                    <RmText type="headline-small" class="mb-4">
                        Representante
                    </RmText>
                    <RmTextField>{{ nomeUsuario }}</RmTextField>

                    <RmDivider class="my-5" />

                    <!-- Vendas -->
                    <RmText type="headline-small">
                        Vendas
                    </RmText>
                    <div class="h-52 relative">
                        <RmMultiChart :types="['line', 'line']" :items="state.dashboard.vendasPorDia ?? []" />
                    </div>

                    <!-- Vendas por categoria -->
                    <RmDivider class="mb-5" />
                    <RmText type="headline-small">
                        Vendas por categoria
                    </RmText>
                    <div class="h-52 relative">
                        <RmDonutChart :items="state.dashboard.vendasPorCategoria ?? []" :display-legend="true" />

                    </div>
                    <RmDivider class="my-5" />

                    <!-- Produtos mais vendidos -->
                    <RmText type="headline-small" class="mb-4">
                        Produtos mais vendidos
                    </RmText>
                    <div>
                        <ItemListaMaisVendidos v-for="(item, index) in state.dashboard.produtosMaisVendidos ?? []"
                                          :key="index"
                                          :item="item" />
                    </div>

                    <RmDivider class="my-5" />

                    <!-- Faturamentos -->
                    <RmText type="headline-small" class="mb-4">
                        Faturamentos
                    </RmText>
                    <table class="red-mobile-table --ultra-small">
                        <tr>
                            <th class="w-16">
                                Pedido
                            </th>
                            <th class="text-left">
                                Cliente
                            </th>
                            <th class="w-16">
                                Data
                            </th>
                            <th class="w-20">
                                Valor faturado
                            </th>
                            <th class="w-28">
                                Valor pedido + IPI
                            </th>
                        </tr>
                        <tr v-for="(faturamento, index) in state.faturamentos" :key="index">
                            <td>{{ faturamento.numeroPedido }}</td>
                            <td class="!text-left">
                                {{ faturamento.nomeCliente }}
                            </td>
                            <td>{{ formatarData(faturamento.dataEmissao ?? '') }}</td>
                            <td>R$ {{ formatarDecimal(faturamento.valorBruto ?? 0) }}</td>
                            <td>R$ {{ formatarDecimal(faturamento.valorBase ?? 0) }}</td>
                        </tr>
                    </table>
                </div>
            </RmPrintPage>
        </Transition>
    </RmPrintView>
</template>
