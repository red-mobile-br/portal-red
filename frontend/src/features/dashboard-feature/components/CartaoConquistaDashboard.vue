<script lang="ts" setup>
import { computed, onMounted, ref, watch } from 'vue';
import PreenchimentoTrofeu from '@/features/metas-feature/pages/historico-metas/components/PreenchimentoTrofeu.vue';
import RmDivider from '@/components/RmDivider.vue';
import RmProgressBar from '@/components/RmProgressBar.vue';
import RmCard from '@/components/RmCard.vue';
import RmText from '@/components/RmText.vue';

import { formatarDecimal } from '@/utils/number-functions';
import { metaService } from '@/services/meta-service';
import { formatarData } from '@/utils/string-functions';
import differenceInCalendarDays from 'date-fns/differenceInCalendarDays';
import MetaListaItemDTO from '@/core/dtos/metas/MetaListaItemDTO';
import format from 'date-fns/format';
import { ptBR } from 'date-fns/locale';

const props = defineProps({
    date: {
        type: Date,
        required: true
    }
});

const meta = ref<MetaListaItemDTO>();
const comErro = ref(false);

async function obterConquistaAtual() {
    try {
        const [ requisicao ] = metaService.obterMetas(
            {
                de: props.date,
            });

        const resposta = await requisicao;
        meta.value = resposta[0];

    } catch (erro) {
        console.log(erro);
        comErro.value = true;
    }
}

// Computeds
const periodo = computed(() => {
    return meta.value ?  `${formatarData(meta.value.dataInicio)} à ${formatarData(meta.value.dataFim)}` : '';
});

const diasRestantes = computed(() => {
    return meta.value ? differenceInCalendarDays(new Date(meta.value.dataFim), new Date()) : 0;
});

const totalDias = computed(() => {
    return meta.value ? differenceInCalendarDays(new Date(meta.value.dataFim), new Date(meta.value.dataInicio)) : 0;
});

const percentualMeta = computed(() => {
    if(!meta.value?.valorRealizado && !meta.value?.valorMeta) return 0;
    return meta.value ? (meta.value.valorRealizado / meta.value.valorMeta) * 100 : 0;
});

const percentualPrazo = computed(() => meta.value ? ((totalDias.value - diasRestantes.value) / totalDias.value) * 100 : 0);

watch(() => props.date, () => obterConquistaAtual(), { deep: true });

onMounted(() => obterConquistaAtual());

</script>

<template>
    <Transition name="fade" mode="out-in">
        <RmCard v-if="comErro">
            Falha ao carregar os dados
        </RmCard>
        <RmCard v-else-if="!meta" />
        
        <RmCard v-else class="!p-0  relative overflow-hidden">
            <RmText type="headline-small" class="px-4 pt-4 pb-2">
                Meta de {{ format(props.date, "MMMM 'de' yyyy", { locale: ptBR }) }}
            </RmText>
            
            <div class="!px-4 flex items-center">
                <PreenchimentoTrofeu :percentage="percentualMeta" />
            
                <div class="px-3 flex-1">
                    <p class="text-sm font-semibold mb-1">
                        {{ meta.titulo }}
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
                            R$ {{ formatarDecimal(meta.valorMeta) }}
                        </p>
                    </div>
            
                    <RmDivider type="vertical" class="h-12" />
                       
                    <div class="px-3 w-32">
                        <p class="text-sm font-semibold mb-1">
                            Valor faturado
                        </p>
                        <p class="text-xs opacity-60">
                            R$ {{ formatarDecimal(meta.valorRealizado) }}
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
                       
            <RmProgressBar class="absolute bottom-0 left-0 w-full" :percentage="percentualPrazo" />
            
        </RmCard>
    </Transition>
</template>