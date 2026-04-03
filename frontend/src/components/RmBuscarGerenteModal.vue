<script lang="ts" setup>
import { reactive } from 'vue';
import { RmIconButton, RmLoading } from '.';
import { Canceler } from 'axios';
import { mascaraCnpj } from '../utils/string-functions';
import RepresentanteListaDTO from '../core/dtos/representante/RepresentanteListaDTO';
import gerenteService from '../services/gerente-service';

export interface BuscarGerenteModalInstancia {
    search: () => Promise<RepresentanteListaDTO | undefined>
}

interface SearchManagerModalState {
    managers: RepresentanteListaDTO[];
    search: string;
    isOpened: boolean;
    loading: boolean;
}

const state = reactive<SearchManagerModalState>({
    isOpened: false,
    managers: [],
    search: '',
    loading: false
});

/** Buscar Gerente */
let requestCanceler: Canceler | null;
const searchManager = () => {
    clearTimeout(searchTimeout);
    requestCanceler && requestCanceler();
    const [ request, canceler ] = gerenteService.obterGerentes({ busca: state.search });

    requestCanceler = canceler;

    request
        .then(resp => {
            state.managers = resp.dados;
        })
        .finally(() => {
            state.loading = false;
        });
};

/** Evento de digitar */
let searchTimeout: any;
const onInput = () => {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(() => {
        if(state.search) {
            searchManager();
        }
    }, 500);
};

/** Buscar gerente */
let promiseResolve: (manager?: RepresentanteListaDTO) => void;
const search = (): Promise<RepresentanteListaDTO | undefined> => {
    return new Promise((resolve) => {
        state.loading = true;
        promiseResolve = resolve;
        searchManager();
        state.isOpened = true;
    });
};

const selectManager = (manager: RepresentanteListaDTO) => {
    state.isOpened = false;
    promiseResolve!(manager);
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
                                <RmIconButton icon="ArrowLeftIcon" icon-class="fill-primary w-8" class="w-8 mx-2" @click="cancel" />
                                <p class="flex-1 font-bold text-left">
                                    Buscar gerente
                                </p>
                            </div>
    
                            <!-- Conteúdo -->
                            <transition name="fade" mode="out-in">
    
                                <div v-if="state.loading" key="0" class="h-96  flex flex-col items-center justify-center">
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
    
                                        <!-- Produtos -->
                                        <tr v-for="(manager, index) in state.managers" :key="index" class="cursor-pointer"
                                            @click="selectManager(manager)">
                                            <td class="px-4">
                                                {{ manager.id }}
                                            </td>
                                            <td class="!text-left !px-3">
                                                {{ manager.nome }}
                                            </td>
                                            <td>{{ mascaraCnpj(manager.cnpj ?? '') }}</td>
                                        </tr>
    
                                        <!-- Nenhum gerente -->
                                        <tr v-if="state.managers?.length == 0">
                                            <td colspan="4">
                                                Nenhum gerente encontrado
                                            </td>
                                        </tr>
                                    </table>
                                </div>
    
                            </transition>
                        </div>
    
                        <!-- Busca -->
                        <form class="h-12 flex items-center pl-5 pr-2" @submit.prevent="searchManager">
                            <input v-model="state.search" 
                                   class="flex-1 bg-transparent py-2 outline-none"
                                   placeholder="Informe o nome ou CNPJ do gerente"
                                   @keydown="onInput">
                            <RmIconButton icon="SearchIcon" icon-class="fill-primary w-8" class="w-8" />
                        </form>
                                
                    </div>
                </transition>
            </div>
        </transition>

    </Teleport>
</template>