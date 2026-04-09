<script lang="ts" setup>
import { computed, PropType } from 'vue';
import { differenceInCalendarDays } from 'date-fns';
import { 
    RmCard,
    RmProgressBar, 
    RmDivider 
} from '@/components';
import PreenchimentoTrofeu from './PreenchimentoTrofeu.vue';

import MetaListaItemDTO from '@/core/dtos/metas/MetaListaItemDTO';
import { formatarData } from '@/utils/string-functions';
import { formatarDecimal } from '@/utils/number-functions';
import { useAutorizacao } from '@/hooks/autorizacao';


const props = defineProps({
    metaAtual: {
        type: Object as PropType<MetaListaItemDTO>,
        required: true
    }
});
const { eGerente } = useAutorizacao();
          
const periodo = computed(() => {
    return `${formatarData(props.metaAtual.dataInicio ?? '')} à ${formatarData(props.metaAtual.dataFim ?? '')}`;
});

const diasRestantes = computed(() => {
    return differenceInCalendarDays(new Date(props.metaAtual.dataFim ?? ''), new Date());
});

const totalDias = computed(() => {
    return differenceInCalendarDays(new Date(props.metaAtual.dataFim ?? ''), new Date(props.metaAtual.dataInicio ?? ''));
});

const percentualMeta = computed(() => {
    if(!props.metaAtual.valorRealizado && !props.metaAtual.valorMeta) return 0;
    return ((props.metaAtual.valorRealizado ?? 0) / (props.metaAtual.valorMeta ?? 0)) * 100;
});

const percentual = computed(() => ((totalDias.value - diasRestantes.value) / totalDias.value) * 100);
</script>

<template>
    <RmCard class="!p-0  relative overflow-hidden">
        <div class="!py-2 !px-4 flex items-center">
            <PreenchimentoTrofeu :percentage="percentualMeta" />

            <div class="px-3 flex-1">
                <p class="text-sm font-semibold mb-1">
                    {{ metaAtual.titulo }}
                </p>
                <p class="text-xs opacity-60">
                    {{ periodo }}
                </p>
            </div>

            <div class="hidden lg:flex items-center">

                <RmDivider type="vertical" class="h-12" />
               
                <div class="px-3 w-32">
                    <p class="text-sm font-semibold mb-1">
                        Valor da meta
                    </p>
                    <p class="text-xs opacity-60">
                        R$ {{ formatarDecimal(metaAtual.valorMeta ?? 0) }}
                    </p>
                </div>

                <RmDivider type="vertical" class="h-12" />

                <div class="px-3 w-32">
                    <p class="text-sm font-semibold mb-1">
                        Valor faturado
                    </p>
                    <p class="text-xs opacity-60">
                        R$ {{ formatarDecimal(metaAtual.valorRealizado ?? 0) }}
                    </p>
                </div>

                <RmDivider type="vertical" class="h-12" />
               
                <div class="px-3 w-32">
                    <p class="text-sm font-semibold mb-1">
                        Prazo restante
                    </p>
                    <p class="text-xs opacity-60">
                        {{ diasRestantes }} dias
                    </p>
                </div>
            </div>
        </div>

        
        <div v-if="eGerente">
            <RmDivider />
          
            <div class="pt-2 pb-4 px-5">
                <p class="text-sm font-semibold mb-1 ">
                    Representante
                </p>
                <p class="text-xs opacity-60">
                    {{ metaAtual.idRepresentante }} - {{ metaAtual.nomeRepresentante }}
                </p>
            </div>
        </div>
        
        <RmProgressBar class="absolute bottom-0 left-0 w-full" :percentage="percentual" />

    </RmCard>
</template>