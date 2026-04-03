<script lang="ts" setup>
import { onMounted, ref } from 'vue';
import { 
    RmBuscarRepresentanteModal,
    BuscarRepresentanteModalInstancia,
    RmCard,
    RmFilterTag,
    RmDateFilterModal,
} from '@/components';

import CartaoConquista from '../historico-metas/components/CartaoConquista.vue';
import { useFiltrosPadrao } from '@/hooks/filtros';
import { useAutorizacao } from '@/hooks/autorizacao';
import MetaListaItemDTO from '@/core/dtos/metas/MetaListaItemDTO';
import { metaService } from '@/services/meta-service';
import { useAlert } from '@/hooks/alerta';
import ErroDTO from '@/core/dtos/ErroDTO';

const carregando = ref(true);
const metas = ref<MetaListaItemDTO[]>([]);
const modalDataAberto = ref(false);
const alert = useAlert();

const { filtros, aplicarFiltros, limparFiltroRepresentante, aplicarFiltrosEReiniciar } = useFiltrosPadrao();
const { eGerente } = useAutorizacao();

// Refs
const refModalBuscarRepresentante = ref<BuscarRepresentanteModalInstancia | null>(null);

async function carregarMetas() {
    carregando.value = true;
    try {
        const [ requisicao ] = metaService.obterMetas({
            idRepresentante: filtros.idRepresentante,
        });

        const resposta = await requisicao;
        metas.value = resposta;
    } catch (e) {
        const erro = e as ErroDTO;
        alert({ mensagem: erro.mensagem });
    }
    finally {
        carregando.value = false;
    }
}

/** Abrir modal de busca de representantes */
const buscarRepresentante = async () => {
    const representante = await refModalBuscarRepresentante.value?.search();
    if(representante) {
        filtros.idRepresentante = representante.id ?? '';
        filtros.nomeRepresentante = representante.nome ?? '';
        aplicarFiltros();
        carregarMetas();
    }
};

onMounted(() => {
    carregarMetas();
});
</script>

<template>

    <div class="w-full max-w-4xl mx-auto">
        <!-- Filtros -->
        <div class="md:col-span-2 xl:col-span-3 flex flex-wrap mb-4">
            <!-- Representante -->
            <RmFilterTag v-if="eGerente"
                         :show-clear-button="filtros.idRepresentante.length > 0"
                         @click="buscarRepresentante"
                         @on:clear="limparFiltroRepresentante(carregarMetas)">
                {{ filtros.nomeRepresentante || "Representante" }}
            </RmFilterTag>
        </div>

            
        <Transition name="fade" mode="out-in">
            <div v-if="carregando" class="flex flex-col items-stretch gap-4">
                <RmCard v-for="n in 5" :key="n" class="h-24 animate-pulse" />
            </div>
            <div v-else-if="metas.length">
                <CartaoConquista v-for="(meta, index) in metas" :key="index" :meta="meta" class="mb-4" />
            </div>
            <p v-else class="text-center text-sm">
                Não há metas
            </p>
        </Transition>

        <!-- Modal de buscar representante -->
        <RmBuscarRepresentanteModal ref="refModalBuscarRepresentante" />

        <RmDateFilterModal v-model:isOpened="modalDataAberto"
                           v-model:dates="filtros.datas"
                           :can-select-future-dates="true"
                           @on:filter="aplicarFiltrosEReiniciar(carregarMetas)" />
    </div>
</template>