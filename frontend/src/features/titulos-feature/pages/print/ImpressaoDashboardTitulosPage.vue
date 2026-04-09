<script lang="ts" setup>
import { computed, onMounted, reactive } from 'vue';
import { formatarDecimal } from '@/utils/number-functions';
import { formatarData, mascaraCnpj } from '@/utils/string-functions';
import statusTituloEnumParser from '@/core/enums/status-titulo-enum-parser';
import format from 'date-fns/format';

// Componentes
import {
    RmPrintView,
    RmPrintPage,
    RmText,
    RmDivider,
    RmTextField,
    RmLoading
} from '@/components';
import GraficoDesempenho from '../dashboard/components/GraficoDesempenho.vue';

// DTOS
import useAppStore from '@/store/app-store';
import TituloListaItemDTO from '@/core/dtos/titulo/TituloListaItemDTO';
import DashboardTitulosDTO from '@/core/dtos/titulo/DashboardTitulosDTO';
import { tituloService } from '@/services/titulo-service';
import { useFiltrosPadrao } from '@/hooks/filtros';


interface EstadoImpressaoTitulos {
    carregando: boolean;
    titulos: TituloListaItemDTO[],
    dashboard: DashboardTitulosDTO
}

const { filtros } = useFiltrosPadrao();
const { nomeUsuario: nome } = useAppStore();
const state = reactive<EstadoImpressaoTitulos>({
    carregando: true,
    titulos: [],
    dashboard: {
        valorRecebido: 0,
        valorAReceber: 0,
        desempenhoPeriodo: []
    }
});

const nomeUsuario = computed(() => filtros.nomeRepresentante || nome.value);
const intervaloData = computed(() => `${format(filtros.datas.start as Date, 'dd/MM/yyyy')} à ${format(filtros.datas.end as Date, 'dd/MM/yyyy')}`);

const carregarDados = async () => {
    try {
        state.carregando = true;

        const [requisicaoTitulos] = tituloService.obterTitulos({
            de: filtros.datas.start as Date,
            ate: filtros.datas.end as Date,
            pagina: filtros.pagina,
            tamanho: 9999,
            status: filtros.status,
            ordenarPor: filtros.ordenarPor,
            direcao: filtros.direcao,
            idRepresentante: filtros.idRepresentante
        });

        const [requisicaoDashboard] = tituloService.obterDashboardTitulos({
            de: filtros.datas.start as Date,
            ate: filtros.datas.end as Date,
            status: filtros.status,
            ordenarPor: filtros.ordenarPor,
            direcao: filtros.direcao,
            idRepresentante: filtros.idRepresentante
        });
        const [respostaTitulos, respostaDashboard] = await Promise.all([requisicaoTitulos, requisicaoDashboard]);

        state.titulos = respostaTitulos.dados;
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

            <RmPrintPage v-else title="Relatório de títulos">
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
                                Valor Recebido:
                            </RmText>
                            <RmText type="display-medium">
                                R$ {{ formatarDecimal(state.dashboard.valorRecebido ?? 0) }}
                            </RmText>
                        </div>
                        <div class="mx-5 w-px bg-gray-300" />
                        <div>
                            <RmText type="headline-small" class="flex-1">
                                Valor a Receber
                            </RmText>
                            <RmText type="display-medium">
                                R$ {{ formatarDecimal(state.dashboard.valorAReceber ?? 0) }}
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

                    <!-- Desempenho -->
                    <RmText type="headline-small" class="mb-4">
                        Desempenho no período
                    </RmText>
                    <div class="h-64">
                        <GraficoDesempenho :items="state.dashboard.desempenhoPeriodo ?? []" />
                    </div>

                    <!-- Titulos -->
                    <RmText type="headline-small" class="mb-4">
                        Títulos
                    </RmText>
                    <table class="red-mobile-table --ultra-small">
                        <tr>
                            <th class="w-16">
                                Título
                            </th>
                            <th class="w-16">
                                Parcela
                            </th>
                            <th class="w-16">
                                Pedido
                            </th>
                            <th class="text-left">
                                Cliente
                            </th>
                            <th class="w-28">
                                CNPJ
                            </th>
                            <th class="w-28">
                                Venc. original
                            </th>
                            <th class="w-28">
                                Venc. real
                            </th>
                            <th class="w-28">
                                D. baixa
                            </th>
                            <th class="w-28">
                                Valor título
                            </th>
                            <th class="w-20">
                                Status
                            </th>
                        </tr>
                        <tr v-for="(titulo, index) in state.titulos" :key="index">
                            <td>{{ titulo.numeroTitulo }}</td>
                            <td>{{ titulo.parcela }}</td>
                            <td>{{ titulo.numeroPedido }}</td>
                            <td class="!text-left">
                                {{ titulo.nomeCliente }}
                            </td>
                            <td>{{ mascaraCnpj(titulo.cnpj ?? '') }}</td>
                            <td>{{ titulo.vencimentoOriginal ? formatarData(titulo.vencimentoOriginal) : '-' }}</td>
                            <td>{{ titulo.dataVencimento ? formatarData(titulo.dataVencimento) : '-' }}</td>
                            <td>{{ titulo.dataPagamento ? formatarData(titulo.dataPagamento) : '-' }}</td>
                            <td>R$ {{ formatarDecimal(titulo.valorTitulo ?? 0) }}</td>
                            <td>
                                {{ statusTituloEnumParser.get(String(titulo.status ?? 0))?.titulo }}
                            </td>
                        </tr>
                    </table>
                </div>
            </RmPrintPage>

        </Transition>
    </RmPrintView>
</template>
