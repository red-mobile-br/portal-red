<script lang="ts" setup>
import { computed, PropType } from 'vue';
import { 
    RmCard,
    RmDivider 
} from '@/components';
import Trofeu from './Trofeu.vue';
import PreenchimentoTrofeu from './PreenchimentoTrofeu.vue';

import MetaListaItemDTO from '@/core/dtos/metas/MetaListaItemDTO';
import { formatarData } from '@/utils/string-functions';
import { formatarDecimal } from '@/utils/number-functions';
import { useAutorizacao } from '@/hooks/autorizacao';

const props = defineProps({
    meta: {
        type: Object as PropType<MetaListaItemDTO>,
        required: true
    }
});

const { eGerente } = useAutorizacao();
const periodo = computed(() => {
    return `${formatarData(props.meta.dataInicio ?? '')} à ${formatarData(props.meta.dataFim ?? '')}`;
});

const percentual = computed(() => (((props.meta.valorRealizado ?? 0) / (props.meta.valorMeta ?? 0)) * 100));

</script>

<template>
    <RmCard class="!p-0 border-l-4 border-gray-200 dark:border-gray-600" :class="{'!border-primary': percentual >= 100}">
        <div class="!py-2 !px-5 flex items-center">
            <component :is="percentual >= 100 ? Trofeu : PreenchimentoTrofeu" :percentage="percentual" />

            <div class="px-3 flex-1">
                <p class="text-sm font-semibold mb-1">
                    {{ meta.titulo }}
                </p>
                <p class="text-xs opacity-60">
                    {{ periodo }}
                </p>
            </div>

            <div class="items-center hidden lg:flex">
                <RmDivider type="vertical" class=" h-12" />
               
                <div class="px-3 w-32">
                    <p class="text-sm font-semibold mb-1 ">
                        Valor da meta
                    </p>
                    <p class="text-xs opacity-60">
                        R$ {{ formatarDecimal(meta.valorMeta ?? 0) }}
                    </p>
                </div>

                <RmDivider type="vertical" class=" h-12" />

                <div class="px-3 w-32">
                    <p class="text-sm font-semibold mb-1 ">
                        Valor faturado
                    </p>
                    <p class="text-xs opacity-60">
                        R$ {{ formatarDecimal(meta.valorRealizado ?? 0) }}
                    </p>
                </div>

                <RmDivider type="vertical" class=" h-12" />
               
                <div v-if="percentual >= 100" class="px-3 flex-1 text-center w-32">
                    <p class="text-sm font-bold mb-1 text-primary">
                        {{ formatarDecimal(percentual) }}%
                    </p>
                    <p class="text-xs opacity-60">
                        do valor faturado
                    </p>
                </div>

                <div v-else class="px-3 flex-1 text-center w-32">
                    <p class="text-sm font-bold mb-1">
                        {{ formatarDecimal(percentual) }}%
                    </p>
                    <p class="text-xs opacity-60">
                        do valor faturado
                    </p>
                </div>
            </div>
        </div>
        
        <div class="lg:hidden">
            <RmDivider />

            <div v-if="eGerente" class="py-2 px-5">
                <p class="text-sm font-semibold mb-1 ">
                    Representante
                </p>
                <p class="text-xs opacity-60">
                    {{ meta.idRepresentante }} - {{ meta.nomeRepresentante }}
                </p>
            </div>

            <div class="py-2 px-5 flex justify-between">
                <div class="w-32">
                    <p class="text-sm font-semibold mb-1 ">
                        Valor da meta
                    </p>
                    <p class="text-xs opacity-60">
                        R$ {{ formatarDecimal(meta.valorMeta ?? 0) }}
                    </p>
                </div>

                <div class="w-32">
                    <p class="text-sm font-semibold mb-1 ">
                        Valor faturado
                    </p>
                    <p class="text-xs opacity-60">
                        R$ {{ formatarDecimal(meta.valorRealizado ?? 0) }}
                    </p>
                </div>
            </div>
        </div>

        <div v-if="eGerente" class="hidden lg:block">
            <RmDivider />
          
            <div class="py-2 px-5">
                <p class="text-sm font-semibold mb-1 ">
                    Representante
                </p>
                <p class="text-xs opacity-60">
                    {{ meta.idRepresentante }} - {{ meta.nomeRepresentante }}
                </p>
            </div>
        </div>
        
    </RmCard>
</template>