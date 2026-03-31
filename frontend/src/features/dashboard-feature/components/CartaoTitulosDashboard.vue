<script lang="ts" setup>
import { onMounted, ref, watch } from 'vue';
import RmCard from '@/components/RmCard.vue';
import RmText from '@/components/RmText.vue';
import RmPaginator from '@/components/RmPaginator.vue';
import TituloListaItemDTO from '@/core/dtos/titulo/TituloListaItemDTO';
import { useFiltrosPadrao } from '@/hooks/filtros';
import { tituloService } from '@/services/titulo-service';
import ListaTitulos from '@/features/titulos-feature/pages/dashboard/components/ListaTitulos.vue';

const props = defineProps({
    start: {
        type: Date,
        required: true
    },
    end: {
        type: Date,
        required: true
    },
});

const STATUS_TITULOS_PENDENTES = '0';

const { filtros } = useFiltrosPadrao();
const titulos = ref<TituloListaItemDTO[]>([]);
const carregando = ref(true);
const comErro = ref(false);


async function obterTitulosDashboard() {
    try {
        carregando.value = true;

        const [requisicao] = tituloService.obterTitulos({
            de: props.start,
            ate: props.end,
            pagina: filtros.pagina,
            tamanho: 12,
            status: STATUS_TITULOS_PENDENTES,
            ordenarPor: filtros.ordenarPor,
            direcao: filtros.direcao,
            idRepresentante: filtros.idRepresentante
        });

        const resposta = await requisicao;
        titulos.value = resposta.dados;
        filtros.totalPaginas = resposta.totalPaginas || 1;
    } catch (error) {
        comErro.value = true;
    } finally {
        carregando.value = false;
    }
}
watch(() => props, () => obterTitulosDashboard(), { deep: true });
onMounted(() => obterTitulosDashboard());


</script>

<template>
    <RmCard>
        <div class="flex items-center mb-4">
            <RmText type="headline-small" class="flex-1">
                Títulos vencidos
            </RmText>
            <RmPaginator v-model:page="filtros.pagina"
                         :total-pages="filtros.totalPaginas"
                         @on:page-change="obterTitulosDashboard" />
        </div>
        <div class="overflow-x-auto">
            <ListaTitulos v-model:orderBy="filtros.ordenarPor"
                        v-model:direction="filtros.direcao"
                        style="min-width: 1000px"
                        :items="titulos"
                        :loading="carregando"
                        @on:order-changed="obterTitulosDashboard" />
        </div>
    </RmCard>
</template>
