<script lang="ts" setup>
import { computed, onMounted, reactive } from 'vue';
import { useRoute } from 'vue-router';
import { formatarData, mascaraCnpj, formatarTitulo } from '@/utils/string-functions';

// Componentes
import {
    RmPrintView,
    RmPrintPage,
    RmText,
    RmDivider,
    RmTextField,
    RmLoading,
    RmDonutChart
} from '@/components';
import ItemListaMelhoresClientes from '../dashboard/components/ItemListaMelhoresClientes.vue';

// DTOS
import useAppStore from '@/store/app-store';
import DashboardClienteDTO from '@/core/dtos/cliente/DashboardClienteDTO';
import ClienteListaItemDTO from '@/core/dtos/cliente/ClienteListaItemDTO';
import { clienteService } from '@/services/cliente-service';
import { useFiltrosPadrao } from '@/hooks/filtros';


interface EstadoImpressaoClientes {
    carregando: boolean;
    dashboard: DashboardClienteDTO;
    clientes: ClienteListaItemDTO[];
    tipoMelhoresClientes: 'orders' | 'revenues'
}

const { query } = useRoute();
const { filtros } = useFiltrosPadrao();
const { nomeUsuario: nome } = useAppStore();
const tipoMelhoresClientes = query['tipoMelhoresClientes'] ? query['tipoMelhoresClientes'] as 'orders' | 'revenues' : 'revenues';

const state = reactive<EstadoImpressaoClientes>({
    carregando: true,
    tipoMelhoresClientes: tipoMelhoresClientes,
    dashboard: {
        clientesPorEstado: [],
        clientesPorStatus: [],
        melhoresClientesPorPedidos: [],
        melhoresClientesPorFaturamento: []
    },
    clientes: [],
});

const nomeUsuario = computed(() => filtros.nomeRepresentante || nome.value);

const totalClientes = computed(() => (state.dashboard.clientesPorEstado ?? []).reduce((acc, el) => acc += (el.quantidadeClientes ?? 0), 0));
const melhoresClientes = computed(() => state.tipoMelhoresClientes == 'orders' ? (state.dashboard.melhoresClientesPorPedidos ?? []) : (state.dashboard.melhoresClientesPorFaturamento ?? []));


const carregarDados = async () => {
    try {
        state.carregando = true;
        const [requisicaoClientes] = clienteService.obterClientes({
            pagina: filtros.pagina,
            tamanho: 9999,
            ordenarPor: filtros.ordenarPor,
            direcao: filtros.direcao,
            status: filtros.status,
            idRepresentante: filtros.idRepresentante
        });

        const [requisicaoDashboard] = clienteService.obterDashboardClientes({
            status: filtros.status,
            ordenarPor: filtros.ordenarPor,
            direcao: filtros.direcao,
            idRepresentante: filtros.idRepresentante
        });

        const [respostaClientes, respostaDashboard] = await Promise.all([requisicaoClientes, requisicaoDashboard]);
        state.clientes = respostaClientes.dados;
        state.dashboard = respostaDashboard;


    } catch (error) {
        window.alert("Não foi possível carregar os dados, por favor, tente novamente");
    }
    finally {
        state.carregando = false;
    }
};


const statusCliente = (status: number) => {
    const todosStatus: { [key: number]: string } = {
        0: 'Sem compras',
        1: 'Ativo',
        2: 'Inativo'
    };
    return todosStatus[status];
};

onMounted(() => carregarDados());
</script>

<template>
    <RmPrintView>
        <Transition name="fade" mode="out-in">
            <div v-if="state.carregando" class="h-screen flex items-center justify-center">
                <RmLoading />
            </div>

            <RmPrintPage v-else title="Relatório de clientes">
                <div class="w-full">

                    <!-- Header -->
                    <div class="flex items-stretch">
                        <div>
                            <RmText type="headline-small" class="flex-1">
                                Clientes atendidos:
                            </RmText>
                            <RmText type="display-medium">
                                {{ totalClientes }}
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

                    <!-- Cliente por status -->
                    <RmText type="headline-small" class="mb-4">
                        Clientes por status
                    </RmText>
                    <div class="h-72 relative">
                        <RmDonutChart :items="state.dashboard.clientesPorStatus ?? []" :display-legend="true" />
                    </div>

                    <RmDivider class="mb-5" />

                    <!-- Maiores clientes -->
                    <RmText type="headline-small" class="mb-4">
                        Maiores clientes
                    </RmText>
                    <table class="red-mobile-table">
                        <tr>
                            <th class="!text-left !px-3">
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
                                               :client="cliente" />

                        <tr v-if="melhoresClientes.length == 0">
                            <td colspan="2">
                                Nenhum cliente encontrado
                            </td>
                        </tr>
                    </table>
                    <RmDivider class="my-5" />

                    <RmText type="headline-small" class="mb-4">
                        Clientes
                    </RmText>
                    <table class="red-mobile-table --ultra-small">
                        <tr>
                            <th class="w-12">
                                Cód.
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
                            <th class="w-24">
                                Cidade
                            </th>
                            <th class="w-8">
                                UF
                            </th>
                            <th class="w-18">
                                Status
                            </th>
                            <th class="w-24">
                                Última compra
                            </th>
                            <th class="w-24">
                                Dias s/ comprar
                            </th>
                        </tr>
                        <tr v-for="(cliente, index) in state.clientes" :key="index">
                            <td>{{ (cliente.id ?? '').substr(0,6) }}</td>
                            <td>{{ (cliente.id ?? '').substr(6,2) }}</td>
                            <td class="!text-left">
                                {{ cliente.nome ?? '' }}
                            </td>
                            <td>{{ mascaraCnpj(cliente.cnpj ?? '') }}</td>
                            <td>{{ formatarTitulo(cliente.cidade ?? '') }}</td>
                            <td>{{ cliente.uf ?? '' }}</td>
                            <td>{{ statusCliente(cliente.status ?? 0) }}</td>
                            <td>{{ cliente.ultimaCompra ? formatarData(cliente.ultimaCompra) : '-' }}</td>
                            <td>{{ cliente.diasSemComprar ?? 0 }}</td>
                        </tr>
                    </table>
                </div>
            </RmPrintPage>
        </Transition>
    </RmPrintView>
</template>
