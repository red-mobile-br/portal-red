<script lang="ts" setup>
import { reactive } from 'vue';
import { RmIconButton, RmLoading, RmPaginator } from '@/components';
import { Canceler } from 'axios';
import { mascaraCnpj } from '@/utils/string-functions';
import { clienteService } from '@/services/cliente-service';
import ClienteListaItemDTO from '@/core/dtos/cliente/ClienteListaItemDTO';

export interface SearchCustomerDialogInstance {
    search: (value: string) => Promise<ClienteListaItemDTO | undefined>
}

interface SearchCustomerDialogState {
    clients: ClienteListaItemDTO[];
    search: string;
    isOpened: boolean;
    loading: boolean;
    page: number;
    totalPages: number;
}

const state = reactive<SearchCustomerDialogState>({
    isOpened: false,
    clients: [],
    search: '',
    loading: false,
    page: 1,
    totalPages: 0
});

/** Buscar Clientes */
let requestCanceler: Canceler | null;
const searchClient = (page: number) => {
    clearTimeout(searchTimeout);
    requestCanceler && requestCanceler();
    const [ request, canceler ] = clienteService.obterClientes({ busca: state.search, pagina: page });
    requestCanceler = canceler;

    request
        .then(resp => {
            state.clients = resp.dados;
            state.page = resp.numeroPagina;
            state.totalPages = Math.max(resp.totalPaginas, 1);
        })
        .finally(() => {
            state.loading = false;
        });
};

const changePage = (page: number) => {
    state.loading = true;
    searchClient(page);
};

/** Evento de digitar */
let searchTimeout: any;
const onInput = () => {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(() => {
        if(state.search) {
            searchClient(1);
        }
    }, 500);
};

let promiseResolve: (client?: ClienteListaItemDTO) => void;
const search = (value: string) => {
    return new Promise((resolve) => {
        state.loading = true;
        promiseResolve = resolve;
        state.search = value;
        searchClient(1);
        state.isOpened = true;
    });
};

const selectClient = (client: ClienteListaItemDTO) => {
    state.isOpened = false;
    promiseResolve!(client);
};

const cancel = () => {
    state.isOpened = false;
    promiseResolve();
};

defineExpose({ search });
</script>

<template>
    <Teleport to="body">
        <transition name="fade">
            <div v-if="state.isOpened" class="fixed top-0 right-0 w-full h-screen bg-black bg-opacity-60 flex items-center justify-center z-20 p-4">

                <transition name="modalAppear" appear>
                    <div class="bg-white dark:bg-gray-700 rounded-lg text-center max-w-2xl w-full">

                        <div class="rounded-lg shadow-lg">

                            <!-- Header -->
                            <div class="flex items-center border-b border-gray-300 dark:border-gray-800 h-14">
                                <RmIconButton icon="ArrowLeftIcon"
                                              icon-class="fill-primary w-8" 
                                              class="w-12"
                                              @click="cancel" />
                                <p class="flex-1 font-bold text-left">
                                    Buscar clientes
                                </p>

                                <RmPaginator v-if="!state.loading"
                                             v-model:page="state.page"
                                             class="mx-2"
                                             :total-pages="state.totalPages"
                                             @on:page-change="changePage" />
                            
                            </div>

                            <!-- Conteúdo -->
                            <transition name="fade" mode="out-in">

                                <div v-if="state.loading" key="0" class="h-96 flex flex-col items-center justify-center">
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
                                            <th class="w-8">
                                                Loja
                                            </th>
                                            <th class="text-left !px-3">
                                                Nome
                                            </th>
                                            <th class="w-44">
                                                Cnpj
                                            </th>
                                        </tr>

                                        <!-- Produtos -->
                                        <tr v-for="(client, index) in state.clients" :key="index" class="cursor-pointer"
                                            @click="selectClient(client)">
                                            <td class="px-4">
                                                {{ client.id.substr(0,6) }}
                                            </td>
                                            <td>{{ client.id.substr(6,2) }}</td>
                                            <td class="!text-left !px-3">
                                                {{ client.nome }}
                                            </td>
                                            <td>{{ mascaraCnpj(client.cnpj) }}</td>
                                        </tr>

                                        <!-- Nenhum produto -->
                                        <tr v-if="state.clients.length == 0">
                                            <td colspan="4">
                                                Nenhum cliente encontrado
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </transition>
                        </div>

                        <!-- Busca -->
                        <form class="h-12 flex items-center pl-5 pr-2" @submit.prevent="() => searchClient(1)">
                            <input v-model="state.search" 
                                   class="flex-1 bg-transparent py-2 outline-none"
                                   placeholder="Informe CNPJ ou nome da empresa"
                                   @keydown="onInput">
                            <RmIconButton icon="SearchIcon" class="fill-primary w-8" />
                        </form>
                            
                    </div>
                </transition>
            </div>
        </transition>
    </Teleport>
</template>
