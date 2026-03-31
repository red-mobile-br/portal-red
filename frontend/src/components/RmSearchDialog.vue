<script lang="ts" setup>
import { PropType, reactive } from 'vue';
import RmLoading  from '@/components/RmLoading.vue';
import RmIconButton from '@/components/RmIconButton.vue';

const props = defineProps({
    title: {
        type: String,
        required: true,
    },
    requestCallback: {
        type: Function as PropType<(filter: string) => Promise<any[]>>,
        required: true,
    },
    placeholder: {
        type: String,
        required: true,
    },
});

export interface RmSearchDialogInstance<T> {
    search: (initialValue: string) => Promise<T | undefined>
}


interface SearchOrderDialogState {
    data: any[];
    search: string;
    isOpened: boolean;
    loading: boolean;
}

const state = reactive<SearchOrderDialogState>({
    isOpened: false,
    data: [],
    search: '',
    loading: false
});

/** Buscar Clientes */
async function searchData() {
    try {
        state.loading = true;
        clearTimeout(searchTimeout);
        state.data = await props.requestCallback(state.search);
        state.loading = false;
        
    } catch (error) {
        return;
    }
}

/** Evento de digitar */
let searchTimeout: any;
const onInput = () => {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(() => {
        searchData();
    }, 500);
};

/** Buscar vendedor */
let promiseResolve: (vendor?: any) => void;
const search = (initialValue: string): Promise<any> => {
    return new Promise((resolve) => {
        state.loading = true;
        state.search = initialValue;
        promiseResolve = resolve;
        searchData();
        state.isOpened = true;
    });
};

const selectData = (vendor: any) => {
    state.isOpened = false;
    promiseResolve!(vendor);
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
                                    {{ props.title }}
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
                                        <slot name="header" />
                                        
    
                                        <!-- Conteúdo -->
                                        <tr v-for="(item, index) in state.data"
                                            :key="index"
                                            class="cursor-pointer"
                                              
                                            @click="() => selectData(item)">
                                            <slot name="content" :data="item" />
                                        </tr>
                                        
                                        <!-- Nenhum produto -->
                                        <slot v-if="state.data.length == 0" name="empty" />
                                        
                                    </table>
                                </div>
    
                            </transition>
                        </div>
    
                        <!-- Busca -->
                        <form class="h-12 flex items-center pl-5 pr-2" @submit.prevent="searchData">
                            <input v-model="state.search" 
                                   class="flex-1 bg-transparent py-2 outline-none"
                                   :placeholder="placeholder"
                                   @keydown="onInput">
                            <RmIconButton icon="SearchIcon" icon-class="fill-primary w-8" class="w-8" />
                        </form>
                                
                    </div>
                </transition>
            </div>
        </transition>
    </Teleport>
</template>