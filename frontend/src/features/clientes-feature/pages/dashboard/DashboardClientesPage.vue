<script lang="ts" setup>
import { computed, onMounted, reactive, ref } from 'vue';
import { Canceler } from 'axios';
import { useRouter } from 'vue-router';
import { useAutorizacao } from '@/hooks/autorizacao';

import {
    RmCard,
    RmText,
    RmDropdown,
    RmDropdownItem,
    RmTextButton,
    RmLoading,
    RmPaginator,
    RmFilterTagButton,
    RmFilterTag,
    RmBuscarRepresentanteModal,
    BuscarRepresentanteModalInstancia,
    RmDonutChart,
    RmDateFilterModal,
} from '@/components';

import MapaClientes from './components/MapaClientes.vue';
import ItemListaMelhoresClientes from './components/ItemListaMelhoresClientes.vue';
import ListaClientes from './components/ListaClientes.vue';

import ClienteListaItemDTO from '@/core/dtos/cliente/ClienteListaItemDTO';
import DashboardClienteDTO from '@/core/dtos/cliente/DashboardClienteDTO';
import statusClienteEnum from '@/core/enums/status-cliente-enum-parser';

import { clienteService } from '@/services/cliente-service';
import { useFiltrosPadrao } from '@/hooks/filtros';
import { AngleDownIcon, ClientesIcon, PrintIcon } from '@/icons';

interface EstadoDashboardClientes {
    carregandoDashboard: boolean;
    carregandoClientes: boolean;
    tipoMelhoresClientes: 'orders' | 'revenues'
    dashboard: DashboardClienteDTO;
    clientes: ClienteListaItemDTO[];
}

const router = useRouter();

const state = reactive<EstadoDashboardClientes>({
    carregandoDashboard: true,
    carregandoClientes: false,
    tipoMelhoresClientes: 'revenues',
    dashboard: {
        clientesPorEstado: [],
        clientesPorEstatus: [],
        melhoresClientesPorPedidos: [],
        melhoresClientesPorFaturamento: []
    },
    clientes: [],
});

// Hooks
const { eGerente } = useAutorizacao();
const { filtros, aplicarFiltrosEReiniciar, limparFiltroStatus, limparFiltroRepresentante, periodo } = useFiltrosPadrao();

// Refs
const refModalBuscarRepresentante = ref<BuscarRepresentanteModalInstancia | null>(null);

// Filtros
const opcoesStatus = Object.entries(statusClienteEnum).map<{ name: string, value: string }>(([chave, el]) => {
    return {
        name: el.titulo,
        value: chave
    };
});

// Propriedades computadas
const totalClientes = computed(() => state.dashboard.clientesPorEstado.reduce((acc, el) => acc += el.quantidadeClientes, 0));
const melhoresClientes = computed(() => state.tipoMelhoresClientes == 'orders' ? state.dashboard.melhoresClientesPorPedidos : state.dashboard.melhoresClientesPorFaturamento);


// Métodos
let canceladorClientes: Canceler | null;

const carregarClientes = (pagina: number) => {
    canceladorClientes && canceladorClientes();
    state.carregandoClientes = true;

    const [requisicao, cancelador] = clienteService.obterClientes({
        pagina: pagina,
        tamanho: filtros.tamanho,
        ordenarPor: filtros.ordenarPor,
        direcao: filtros.direcao,
        status: filtros.status,
        idRepresentante: filtros.idRepresentante
    });
    canceladorClientes = cancelador;
    requisicao
        .then(resp => {
            state.clientes = resp.dados;
            filtros.totalPaginas = resp.totalPaginas;
            state.carregandoClientes = false;
        });
};

/** Carregar dados do dashboard */
const carregarDados = async () => {
    try {
        state.carregandoDashboard = true;

        const [requisicaoClientes] = clienteService.obterClientes({
            pagina: filtros.pagina,
            tamanho: 12,
            ordenarPor: filtros.ordenarPor,
            direcao: filtros.direcao,
            status: filtros.status,
            idRepresentante: filtros.idRepresentante
        });

        const [requisicaoDashboard] = clienteService.obterDashboardClientes({
            de: filtros.datas.start,
            ate: filtros.datas.end,
            status: filtros.status,
            ordenarPor: filtros.ordenarPor,
            direcao: filtros.direcao,
            idRepresentante: filtros.idRepresentante
        });

        const [respostaClientes, respostaDashboard] = await Promise.all([requisicaoClientes, requisicaoDashboard]);
        state.clientes = respostaClientes.dados;
        state.dashboard = respostaDashboard;

        filtros.totalPaginas = respostaClientes.totalPaginas;

    } catch (error) {
        window.alert("Não foi possível carregar os dados, por favor, tente novamente");
    }
    finally {
        state.carregandoDashboard = false;
    }
};

const imprimir = () => {
    const rota = router.resolve({
        name: 'impressaoClientes',
        query: {
            tipoMelhoresClientes: state.tipoMelhoresClientes,
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

onMounted(() => carregarDados());
</script>

<template>
    <transition name="fade" mode="out-in">

        <div v-if="state.carregandoDashboard" key="1" class="h-novo-pedido-card flex items-center justify-center">
            <RmLoading />
        </div>

        <div v-else key="2" class="flex-1 grid gap-4 clientes-dashboard pb-4">
            <!-- Filtros -->
            <div class="flex flex-wrap md:col-span-2 -mb-2">

                <!-- Status -->
                <RmFilterTagButton v-model="filtros.status"
                                   :options="opcoesStatus"
                                   :show-clear-button="filtros.status.length > 0"
                                   placeholder="Status"
                                   @on:change="aplicarFiltrosEReiniciar(carregarDados)"
                                   @on:clear="limparFiltroStatus(carregarDados)" />

                <!-- Período -->
                <RmFilterTag @click="filtros.modalDataAberto = true">
                    {{ periodo }}
                </RmFilterTag>


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

            <RmCard class="overflow-hidden !p-0 relative">
                <MapaClientes :clients="state.dashboard.clientesPorEstado" />

                <div class="w-2/3 absolute top-0 left-0 h-full bg-gradient-to-r from-white to-transparent opacity-80 pointer-events-none" />
                <div class="absolute top-4 left-4">
                    <div class="flex items-center mb-2">
                        <ClientesIcon class="fill-primary w-8" />
                        <div class="font-light text-2xl opacity-80 mx-1 text-neutral-900">
                            {{ totalClientes }}
                        </div>
                    </div>
                    <RmText type="headline-small" class="!text-neutral-900">
                        Clientes atendidos
                    </RmText>
                </div>
            </RmCard>

            <!-- Status cliente -->
            <RmCard class="flex flex-col items-stretch">
                <RmText type="headline-small" class="mb-2 relative">
                    Status clientes
                </RmText>
                <div class="flex-1 relative">
                    <RmDonutChart :items="state.dashboard.clientesPorEstatus" :display-legend="true" />
                </div>
            </RmCard>

            <!-- Clientes -->
            <RmCard class="md:col-span-2 2xl:col-span-1">
                <div class="flex items-center mb-4">
                    <RmText type="headline-small" class="flex-1">
                        Clientes
                    </RmText>

                    <RmPaginator v-model:page="filtros.pagina"
                                 :total-pages="filtros.totalPaginas"
                                 @on:page-change="carregarClientes" />
                </div>

                <div class="overflow-x-auto light-scroll w-full">
                    <ListaClientes v-model:orderBy="filtros.ordenarPor"
                                   v-model:direction="filtros.direcao"
                                   :loading="state.carregandoClientes"
                                   :items="state.clientes"
                                   @on:order-changed="carregarClientes"
                                   @on:select-customer="$router.push({ name: 'consultar-cliente', params: {cnpj: $event}})" />
                </div>
            </RmCard>

            <!-- Maiores clientes -->
            <RmCard class="md:col-span-2 2xl:col-span-1">

                <div class="flex items-center justify-between mb-4">
                    <RmText type="headline-small">
                        Maiores clientes
                    </RmText>

                    <RmDropdown>
                        <template #default>
                            <RmTextButton>
                                <RmText type="body-small" class="mr-1">
                                    Por {{ state.tipoMelhoresClientes == 'orders' ? 'número de pedidos' : 'faturamento' }}
                                </RmText>
                                <AngleDownIcon class="w-3 fill-primary" />
                            </RmTextButton>
                        </template>

                        <template #content>
                            <RmDropdownItem icon="CheckIcon"
                                            label="Faturamento"
                                            :show-icon="state.tipoMelhoresClientes == 'revenues'"
                                            @click="state.tipoMelhoresClientes = 'revenues'" />

                            <RmDropdownItem icon="CheckIcon"
                                            :show-icon="state.tipoMelhoresClientes == 'orders'"
                                            label="Pedidos"
                                            @click="state.tipoMelhoresClientes = 'orders'" />
                        </template>
                    </RmDropdown>
                </div>

                <table class="red-mobile-table">
                    <tr>
                        <th class="!text-left">
                            Cliente
                        </th>
                        <th class="w-32 text-center">
                            {{ state.tipoMelhoresClientes == 'orders' ? 'Pedidos' : 'Faturamento' }}
                        </th>
                    </tr>

                    <ItemListaMelhoresClientes v-for="(cliente, index) in melhoresClientes"
                                           :key="index"
                                           :is-revenue="state.tipoMelhoresClientes == 'revenues'"
                                           :index="index"
                                           :client="cliente"
                                           @click="$router.push({ name: 'consultar-cliente', params: {cnpj: cliente.cnpj}})" />

                    <tr v-if="melhoresClientes.length == 0">
                        <td colspan="2">
                            Nenhum cliente encontrado
                        </td>
                    </tr>
                </table>
            </RmCard>

            <!-- Modal de buscar representante -->
            <RmBuscarRepresentanteModal ref="refModalBuscarRepresentante" />


            <RmDateFilterModal v-model:isOpened="filtros.modalDataAberto"
                               v-model:dates="filtros.datas"
                               :can-select-future-dates="true"
                               @on:filter="aplicarFiltrosEReiniciar(carregarDados)" />

        </div>

    </transition>
</template>

<style>
.clientes-dashboard {
    grid-template-columns: repeat(1, minmax(0, 1fr));
    grid-template-rows: auto 450px 350px 1fr auto;
    grid-auto-rows: 1fr ;
}

@media (min-width: 768px) {
    .clientes-dashboard {
        grid-template-columns: repeat(2, minmax(0, 1fr));
        grid-template-rows: auto 350px 1fr auto;
    }
}

@media (min-width: 1280px) {
    .clientes-dashboard {
        grid-template-columns: minmax(0, 1fr) 350px;
        grid-template-rows: auto 350px;
    }
}
@media (min-width: 1536px) {
    .clientes-dashboard {
        grid-template-columns: minmax(0, 1fr) 450px;
        grid-template-rows: auto 450px;
    }
}
</style>
