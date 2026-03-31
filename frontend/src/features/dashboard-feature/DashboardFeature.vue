<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';

import RmAdminPage from '@/components/RmAdminPage.vue';
import RmText from '@/components/RmText.vue';
import RmCard from '@/components/RmCard.vue';
import RmDivider from '@/components/RmDivider.vue';
import RmDonutChart from '@/components/charts/RmDonutChart.vue';
import RmAreaChart from '@/components/charts/RmAreaChart.vue';
import RmFilterTag from '@/components/RmFilterTag.vue';
import RmBuscarRepresentanteModal from '@/components/RmBuscarRepresentanteModal.vue';
import { BuscarRepresentanteModalInstancia } from '@/components/RmBuscarRepresentanteModal.vue';
import RmMonthPicker from '@/components/RmMonthPicker.vue';

import CartaoConquistaDashboard from './components/CartaoConquistaDashboard.vue';
import CartaoTitulosDashboard from './components/CartaoTitulosDashboard.vue';
import CartaoPedidosDashboard from './components/CartaoPedidosDashboard.vue';

import { formatarDecimal } from '@/utils/number-functions';
import { DashboardDTO } from '@/core/dtos/dashboard/dashboard-dto';
import { dashboardService } from '@/services/dashboard-service';
import { useFiltrosPadrao } from '@/hooks/filtros';
import { useAutorizacao } from '@/hooks/autorizacao';

const { filtros, aplicarFiltrosEReiniciar, limparFiltroRepresentante, filtroMes } = useFiltrosPadrao();
const { eGerente } = useAutorizacao();

const refModalBuscarRepresentante = ref<BuscarRepresentanteModalInstancia>();
const dadosDashboard = ref<DashboardDTO>();
const carregando = ref(true);

/** Abrir modal de busca de representantes */
const buscarRepresentante = async () => {
    const representante = await refModalBuscarRepresentante.value?.search();
    if(representante) {
        filtros.idRepresentante = representante.id;
        filtros.nomeRepresentante = representante.nome;
        aplicarFiltrosEReiniciar(carregarDados);
    }
};

async function carregarDados() {
    try {
        carregando.value = true;
        const [requisicao] = dashboardService.obterDashboard({
            de: filtros.datas.start,
            ate: filtros.datas.end,
            tamanho: filtros.tamanho,
            ordenarPor: filtros.ordenarPor,
            direcao: filtros.direcao,
            status: filtros.status,
            idRepresentante: filtros.idRepresentante
        });
        dadosDashboard.value = await requisicao;
    } catch (error) {
        window.alert("Não foi possível carregar os dados");
    }
    finally {
        carregando.value = false;
    }
}

const clientesPorEstadoParaGrafico = computed(() => {
    if (!dadosDashboard.value?.clientesPorEstado) return [];
    return dadosDashboard.value.clientesPorEstado.map(item => ({
        rotulo: item.estado,
        series: [{ nome: item.estado, valor: item.quantidadeClientes }]
    }));
});

onMounted(() => carregarDados());
</script>

<template>
    <RmAdminPage class="flex flex-col items-stretch pb-4">
        <!-- Dashboard -->
        <RmText type="display-medium" class="border-b border-gray-300 dark:border-gray-600 mb-4 pb-2">
            Dashboard
        </RmText>

        <!-- Filtros -->
        <div class="flex flex-wrap mb-2">

            <RmMonthPicker v-model="filtroMes"
                           @on:date-change="() => aplicarFiltrosEReiniciar(carregarDados)" />

            <!-- Representante -->
            <RmFilterTag v-if="eGerente"
                         :show-clear-button="!!filtros.idRepresentante"
                         @click="buscarRepresentante"
                         @on:clear="limparFiltroRepresentante(carregarDados)">
                {{ filtros.nomeRepresentante || "Representante" }}
            </RmFilterTag>
        </div>

        <div class="grid dashboard gap-4 flex-1">

            <!-- Comissão durante o período -->
            <Transition name="fade" mode="out-in">
                <RmCard v-if="carregando || !dadosDashboard" />
                <RmCard v-else>
                    <RmText type="headline-small" class="mb-2">
                        Valor total de comissão
                    </RmText>
                    <p class="font-semibold text-2xl text-primary-light">
                        R$ {{ formatarDecimal(dadosDashboard.totalComissaoPeriodo) }}
                    </p>
                    <RmDivider class="mb-2 my-3" />
                    <RmText type="label-small">
                        Durante o período
                    </RmText>
                </RmCard>
            </Transition>

            <!-- Total de pedidos -->
            <Transition name="fade" mode="out-in">
                <RmCard v-if="carregando || !dadosDashboard" />
                <RmCard v-else>
                    <RmText type="headline-small" class=" mb-2">
                        Total de pedidos
                    </RmText>
                    <p class="font-semibold text-2xl text-primary-light">
                        {{ dadosDashboard.totalPedidosPeriodo }}
                    </p>
                    <RmDivider class="mb-2 my-3" />
                    <RmText type="label-small">
                        Durante o período
                    </RmText>
                </RmCard>
            </Transition>


            <!-- Pedidos em aberto -->
            <Transition name="fade" mode="out-in">
                <RmCard v-if="carregando || !dadosDashboard" />
                <RmCard v-else>
                    <RmText type="headline-small" class="mb-2">
                        Pedidos em aberto
                    </RmText>
                    <p class="font-semibold text-2xl text-accent">
                        {{ dadosDashboard.pedidosAbertosPeriodo }}
                    </p>
                    <RmDivider class="mb-2 my-3" />
                    <RmText type="label-small">
                        Durante o período
                    </RmText>
                </RmCard>
            </Transition>

            <!-- Status dos clientes -->
            <Transition name="fade" mode="out-in">
                <RmCard v-if="carregando || !dadosDashboard" class="md:col-span-3 xl:col-span-1 xl:row-span-3" />
                <RmCard v-else class="md:col-span-3 xl:col-span-1 xl:row-span-3 flex flex-col items-stretch">
                    <RmText type="headline-small" class="mb-2 relative">
                        Status clientes
                    </RmText>
                    <div class="flex-1 relative">
                        <RmDonutChart :items="clientesPorEstadoParaGrafico" :display-legend="true" />
                    </div>
                </RmCard>
            </Transition>

            <!-- Desempenho no período -->
            <Transition name="fade" mode="out-in">
                <RmCard v-if="carregando || !dadosDashboard" class="md:col-span-3 flex flex-col items-stretch" />
                <RmCard v-else class="md:col-span-3 flex flex-col items-stretch">
                    <RmText type="headline-small" class="mb-4">
                        Desempenho no período
                    </RmText>
                    <div class="flex-1 transition-opacity duration-300 relative">
                        <RmAreaChart :items="dadosDashboard.comissoesPeriodo" />
                    </div>
                </RmCard>
            </Transition>

            <!-- Conquistas -->
            <CartaoConquistaDashboard :date="filtroMes" class="md:col-span-3" />


            <CartaoPedidosDashboard :start="filtros.datas.start" :end="filtros.datas.end" class="md:col-span-3 xl:col-span-4" />


            <!-- Títulos vencidos -->
            <CartaoTitulosDashboard :start="filtros.datas.start" :end="filtros.datas.end" class="md:col-span-3 xl:col-span-4" />
        </div>


        <!-- Modal de buscar representante -->
        <RmBuscarRepresentanteModal ref="refModalBuscarRepresentante" />
    </RmAdminPage>
</template>

<style scoped>
.dashboard {
    grid-template-columns: minmax(0, 1fr);
    grid-template-rows: 130px 130px 130px 300px 300px 140px;
}

@media (min-width: 768px) {
    .dashboard {
        grid-template-columns: minmax(0, 1fr) minmax(0, 1fr) minmax(0, 1fr);
        grid-template-rows: 130px 300px 300px 140px;
    }
}
@media (min-width: 1280px) {
    .dashboard {
        grid-template-columns: minmax(0, 1fr) minmax(0, 1fr) minmax(0, 1fr) 22rem;
        grid-template-rows: 8.5rem 12.5rem 8.5rem minmax(20rem,1fr);
    }
}

</style>
