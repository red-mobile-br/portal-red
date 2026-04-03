<script lang="ts" setup>
import { computed, onMounted, reactive } from 'vue';
import { formatarDecimal, arredondarPreco } from '@/utils/number-functions';
import { formatarData } from '@/utils/string-functions';

import format from 'date-fns/format';

// Componentes
import {
    RmPrintView,
    RmPrintPage,
    RmText,
    RmDivider,
    RmTextField,
    RmLoading,
    RmBarChart,
    RmAreaChart,
} from '@/components';


// DTOS
import useAppStore from '@/store/app-store';
import DashboardComissoesDTO from '@/core/dtos/comissoes/DashboardComissoesDTO';
import ComissaoListaItemDTO from '@/core/dtos/comissoes/ComissaoListaItemDTO';
import comissaoService from '@/services/comissao-service';
import { useFiltrosPadrao } from '@/hooks/filtros';

interface EstadoImpressaoComissoes {
    carregando: boolean;
    dashboard: DashboardComissoesDTO;
    comissoes: ComissaoListaItemDTO[];
}

const { filtros } = useFiltrosPadrao();
const { nomeUsuario: nome } = useAppStore();

const state = reactive<EstadoImpressaoComissoes>({
    carregando: true,
    dashboard: {
        totalComissaoPeriodo: 0,
        percentualComissaoPeriodo: 0,
        comissoesPeriodo: [],
        maioresComissoesPeriodo: []
    },
    comissoes: [],
});

const nomeUsuario = computed(() => filtros.nomeRepresentante || nome.value);
const intervaloData = computed(() => `${format(filtros.datas.start as Date, 'dd/MM/yyyy')} à ${format(filtros.datas.end as Date, 'dd/MM/yyyy')}`);

const carregarDados = async () => {
    try {
        state.carregando = true;
        const [requisicaoComissoes] = comissaoService.obterComissoes({
            de: filtros.datas.start as Date,
            ate: filtros.datas.end as Date,
            pagina: filtros.pagina,
            tamanho: 9999,
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

            <RmPrintPage v-else title="Relatório de comissões">
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
                                Valor total de comissão:
                            </RmText>
                            <RmText type="display-medium">
                                R$ {{ formatarDecimal(state.dashboard.totalComissaoPeriodo ?? 0) }}
                            </RmText>
                        </div>
                        <div class="mx-5 w-px bg-gray-300" />
                        <div>
                            <RmText type="headline-small" class="flex-1">
                                Porcentagem média
                            </RmText>
                            <RmText type="display-medium">
                                {{ formatarDecimal(arredondarPreco(state.dashboard.percentualComissaoPeriodo ?? 0)) }}%
                            </RmText>
                        </div>
                    </div>

                    <RmDivider class="my-5" />

                    <!-- Representante -->
                    <RmText type="headline-small" class="mb-4">
                        Representante
                    </RmText>
                    <RmTextField>{{ nomeUsuario }}</RmTextField>

                    <!-- Desempenho do período -->
                    <RmDivider class="my-5" />
                    <RmText type="headline-small">
                        Desempenho no período
                    </RmText>
                    <div class="h-64 relative">
                        <RmAreaChart :items="state.dashboard.comissoesPeriodo ?? []" />
                    </div>

                    <!-- Maiores comissões no período-->
                    <RmDivider class="mb-5" />
                    <RmText type="headline-small">
                        Maiores comissões no período
                    </RmText>
                    <div class="h-64 relative">
                        <RmBarChart :items="state.dashboard.maioresComissoesPeriodo ?? []" :step-size="50" />
                    </div>

                    <RmDivider class="mb-5" />
                    <RmText type="headline-small" class="mb-4">
                        Comissões
                    </RmText>
                    <table class="red-mobile-table --ultra-small">
                        <tr>
                            <th class="w-12">
                                Pedido
                            </th>
                            <th class="text-left">
                                Cliente
                            </th>
                            <th class="w-16">
                                Título
                            </th>
                            <th class="w-12">
                                Parcela
                            </th>
                            <th class="w-20">
                                Venc. original
                            </th>
                            <th class="w-20">
                                Data baixa
                            </th>
                            <th class="w-20">
                                Valor título
                            </th>
                            <th class="w-20">
                                B. de cálculo
                            </th>
                            <th class="w-10">
                                %
                            </th>
                            <th class="w-20">
                                Comissão
                            </th>
                        </tr>
                        <tr v-for="(comissao, index) in state.comissoes" :key="index">
                            <td>{{ comissao?.pedido ?? '' }}</td>
                            <td class="!text-left">
                                {{ comissao?.nomeCliente ?? '' }}
                            </td>
                            <td>{{ comissao?.titulo ?? '' }}</td>
                            <td>{{ comissao?.parcela ?? '' }}</td>
                            <td>{{ comissao?.dataVencimento ? formatarData(comissao.dataVencimento) : '-' }}</td>
                            <td>{{ comissao?.dataBaixa ? formatarData(comissao.dataBaixa) : '-' }}</td>
                            <td>R$ {{ formatarDecimal(comissao?.valorTitulo ?? 0) }}</td>
                            <td>R$ {{ formatarDecimal(comissao?.valorBase ?? 0) }}</td>
                            <td>{{ formatarDecimal(comissao?.percentualComissao ?? 0) }}%</td>
                            <td>R$ {{ formatarDecimal(comissao?.valorComissao ?? 0) }}</td>
                        </tr>
                    </table>

                </div>
            </RmPrintPage>
        </Transition>
    </RmPrintView>
</template>
