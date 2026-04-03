<script lang="ts" setup>
import { reactive } from 'vue';
import { RmIconButton, RmLoading } from '.';
import { Canceler } from 'axios';
import { mascaraCnpj } from '../utils/string-functions';
import RepresentanteListaDTO from '../core/dtos/representante/RepresentanteListaDTO';
import { clienteService } from '../services/cliente-service';

export interface BuscarClienteModalInstancia {
    search: () => Promise<RepresentanteListaDTO | undefined>
}

interface EstadoBuscarClienteModal {
    clientes: RepresentanteListaDTO[];
    busca: string;
    aberto: boolean;
    carregando: boolean;
}

const state = reactive<EstadoBuscarClienteModal>({
    aberto: false,
    clientes: [],
    busca: '',
    carregando: false
});

/** Buscar clientes */
let canceladorRequisicao: Canceler | null;
const buscarCliente = () => {
    clearTimeout(temporizadorBusca);
    canceladorRequisicao && canceladorRequisicao();
    const [requisicao, cancelador] = clienteService.buscarClientes({ busca: state.busca });

    canceladorRequisicao = cancelador;

    requisicao
        .then(resp => {
            state.clientes = resp.dados;
        })
        .finally(() => {
            state.carregando = false;
        });
};

/** Evento de digitar */
let temporizadorBusca: any;
const aoDigitar = () => {
    clearTimeout(temporizadorBusca);
    temporizadorBusca = setTimeout(() => {
        if(state.busca) {
            buscarCliente();
        }
    }, 500);
};

/** Abrir modal e retornar cliente selecionado */
let resolverPromessa: (client?: RepresentanteListaDTO) => void;
const search = (): Promise<RepresentanteListaDTO | undefined> => {
    return new Promise((resolve) => {
        state.carregando = true;
        resolverPromessa = resolve;
        buscarCliente();
        state.aberto = true;
    });
};

const selecionarCliente = (cliente: RepresentanteListaDTO) => {
    state.aberto = false;
    resolverPromessa!(cliente);
};

const cancelar = () => {
    state.aberto = false;
    resolverPromessa();
};

defineExpose({ search });
</script>

<template>
    <Teleport to="body">
        <transition name="fade">
            <div v-if="state.aberto" class="fixed top-0 right-0 w-full h-screen bg-black bg-opacity-60 flex items-center justify-center z-20 p-4">

                <transition name="modalAppear" appear>
                    <div class="bg-white dark:bg-gray-700 rounded-lg text-center max-w-2xl w-full">

                        <div class="rounded-lg shadow-lg">

                            <!-- Header -->
                            <div class="flex items-center border-b border-gray-300 dark:border-gray-800 h-14">
                                <RmIconButton icon="ArrowLeftIcon" icon-class="fill-primary w-8" class="w-8 mx-2" @click="cancelar" />
                                <p class="flex-1 font-bold text-left">
                                    Buscar cliente
                                </p>
                            </div>

                            <!-- Conteúdo -->
                            <transition name="fade" mode="out-in">

                                <div v-if="state.carregando" key="0" class="h-96  flex flex-col items-center justify-center">
                                    <RmLoading class="mb-4" />
                                    <p class="opacity-60">
                                        Carregando
                                    </p>
                                </div>

                                <div v-else key="1" class="h-96 p-4 light-scroll overflow-auto">
                                    <table class="red-mobile-table">
                                        <!-- Header -->
                                        <tr>
                                            <th class="w-20 !px-4">
                                                Código
                                            </th>
                                            <th class="text-left !px-3">
                                                Nome
                                            </th>
                                            <th class="w-44">
                                                CNPJ
                                            </th>
                                        </tr>

                                        <!-- Clientes -->
                                        <tr v-for="(cliente, index) in state.clientes" :key="index" class="cursor-pointer"
                                            @click="selecionarCliente(cliente)">
                                            <td class="px-4">
                                                {{ cliente.id }}
                                            </td>
                                            <td class="!text-left !px-3">
                                                {{ cliente.nome }}
                                            </td>
                                            <td>{{ mascaraCnpj(cliente.cnpj ?? '') }}</td>
                                        </tr>

                                        <!-- Nenhum cliente -->
                                        <tr v-if="state.clientes?.length == 0">
                                            <td colspan="4">
                                                Nenhum cliente encontrado
                                            </td>
                                        </tr>
                                    </table>
                                </div>

                            </transition>
                        </div>

                        <!-- Busca -->
                        <form class="h-12 flex items-center pl-5 pr-2" @submit.prevent="buscarCliente">
                            <input v-model="state.busca"
                                   class="flex-1 bg-transparent py-2 outline-none"
                                   placeholder="Informe o nome ou CNPJ do cliente"
                                   @keydown="aoDigitar">
                            <RmIconButton icon="SearchIcon" icon-class="fill-primary w-8" class="w-8" />
                        </form>

                    </div>
                </transition>
            </div>
        </transition>

    </Teleport>
</template>
