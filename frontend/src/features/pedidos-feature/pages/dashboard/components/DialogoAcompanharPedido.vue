<script lang="ts" setup>
import { PropType } from 'vue';
import { useRouter } from 'vue-router';

import RmIconButton from '../../../../../components/RmIconButton.vue';
import RmTimelineItem from '../../../../../components/RmTimelineItem.vue';

import PedidoListaItemDTO from '../../../../../core/dtos/pedido/PedidoListaItemDTO';

const props = defineProps({
    order: {
        type: Object as PropType<PedidoListaItemDTO | null>,
    },
    isQuotePage: {
        type: Boolean,
    }
});

const emits = defineEmits(['update:order']);

const router = useRouter();

const checkDetail = () => {
    const orderId = props.order?.id;
    emits('update:order', null);
    setTimeout(() => {
        router.push({ name: props.isQuotePage ? 'consultarOrcamento' : 'consultarPedido', params: { id: orderId?.toString() || "" } });
    }, 300);
};

const title = props.isQuotePage ? 'orçamento' : 'pedido';

</script>

<template>
    <Teleport to="body">
        <Transition name="fade">
            <div v-if="order != null" 
                 class="fixed top-0 right-0 w-full h-screen bg-black bg-opacity-60 flex items-center justify-center z-30 p-4">
                <Transition name="modalAppear" appear>
    
                    <div class="bg-white dark:bg-gray-700 rounded-lg text-center max-w-2xl w-full">
    
                        <div class="rounded-lg shadow-lg">
    
                            <!-- Header -->
                            <div class="flex items-center border-b border-gray-300 dark:border-gray-800 h-14">
                                <RmIconButton icon="ArrowLeftIcon"  
                                              icon-class="w-8 fill-primary"
                                              @click="$emit('update:order', null)" />
                                <p class="flex-1 font-bold text-left">
                                    Acompanhamento do {{ title }}
                                </p>
                            </div>
    
    
                            <div v-if="order.anotacoes && order.anotacoes.length > 0" class="h-96 p-4 light-scroll overflow-auto">
                                <RmTimelineItem v-for="(note, index) in order.anotacoes" :key="index"
                                                :date="note.dataHora ?? ''"
                                                :title="note.atividade ?? ''"
                                                :subtitle="note.detalhe ?? ''">
                                    <p class="text-sm">
                                        {{ note.autor }}
                                    </p>
                                </RmTimelineItem>
                            </div>
                            
                            <div v-else class="h-96 flex items-center justify-center">
                                <p class="text-sm">
                                    Não houve atualizações para este {{ title }}
                                </p>
                            </div>
    
                        </div>
    
                        <!-- Busca -->
                        <button class="h-12 flex items-center justify-center w-full" @click="checkDetail">
                            <p class="text-primary font-bold text-sm">
                                Detalhes do {{ title }}
                            </p>
                        </button>
                                
                    </div>
                </Transition>
            </div>
        </Transition>
    </Teleport>
</template>