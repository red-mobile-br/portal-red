<script lang="ts" setup>
import { reactive } from 'vue';
import { Canceler } from 'axios';

import { RmIconButton, RmLoading } from '../../../../../components';
import ProdutoDTO from '@/core/dtos/produto/ProdutoDTO';
import { clienteService } from '@/services/cliente-service';
import { formatarDecimal } from '@/utils/number-functions';

export interface SearchProductDialogInstance {
    search: (value: string) => Promise<ProdutoDTO | undefined>
}

interface SearchProductDialogState {
    products: ProdutoDTO[];
    search: string;
    isOpened: boolean;
    loading: boolean;
}

const props = defineProps({
    selectedClientId: {
        type: String,
        required: true
    }
});

const state = reactive<SearchProductDialogState>({
    isOpened: false,
    products: [],
    search: '',
    loading: false
});


/** Buscar Clientes */
let requestCanceler: Canceler | null;
const searchProduct = () => {
    clearTimeout(searchTimeout);
    requestCanceler && requestCanceler();
    const [ request, canceler ] = clienteService.obterProdutosCliente(props.selectedClientId, { busca: state.search  });
    requestCanceler = canceler;

    request
        .then(resp => {
            state.products = resp.dados;
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
            searchProduct();
        }
    }, 500);
};

let promiseResolve: (client?: ProdutoDTO) => void;
const search = (value: string) => {
    return new Promise((resolve) => {
        state.loading = true;
        promiseResolve = resolve;
        state.search = value;
        searchProduct();
        state.isOpened = true;
    });
};

const selectProduct = (client: ProdutoDTO) => {
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
            <div v-if="state.isOpened" class="fixed top-0 right-0 w-full h-screen bg-black bg-opacity-60 flex items-center justify-center z-30 p-4">
                <transition name="modalAppear" appear>
                    
                    <div class="bg-white dark:bg-gray-700 rounded-lg text-center max-w-2xl w-full">

                        <div class="rounded-lg shadow-lg">

                            <!-- Header -->
                            <div class="flex items-center border-b border-gray-300 dark:border-gray-800 h-14">
                                <RmIconButton icon="ArrowLeftIcon"
                                              class="fill-primary w-12" 
                                              icon-class="w-8"
                                              @click="cancel" />
                                <p class="flex-1 font-bold text-left">
                                    Buscar produto
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
                                            <th class="w-20">
                                                Código
                                            </th>
                                            <th class="text-left">
                                                Nome
                                            </th>
                                            <th class="w-20">
                                                Preço
                                            </th>
                                        </tr>

                                        <!-- Produtos -->
                                        <tr v-for="(product, index) in state.products" :key="index" class="cursor-pointer"
                                            @click="selectProduct(product)">
                                            <td>{{ product.id }}</td>
                                            <td class="!text-left">
                                                {{ product.descricao }}
                                            </td>
                                            <td>R$ {{ formatarDecimal(product.valorUnitario ?? 0) }}</td>
                                        </tr>

                                        <!-- Nenhum produto -->
                                        <tr v-if="state.products.length == 0">
                                            <td colspan="2">
                                                Nenhum produto encontrado
                                            </td>
                                        </tr>
                                    </table>
                                </div>

                            </transition>
                        </div>

                        <!-- Busca -->
                        <form class="h-12 flex items-center pl-5 pr-2" @submit.prevent="searchProduct">
                            <input v-model="state.search" 
                                   class="flex-1 bg-transparent py-2 outline-none"
                                   placeholder="Informe código ou nome do produto"
                                   @keydown="onInput">
                            <RmIconButton icon="SearchIcon" icon-class="w-8 fill-primary" class="w-8" />
                        </form>
                                
                    </div>
                </transition>
            </div>
        </transition>
    </Teleport>
   
</template>