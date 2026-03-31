<script lang="ts" setup>
import { onMounted, ref, watch } from 'vue';

// Services and dtos
import PedidoListaItemDTO from '@/core/dtos/pedido/PedidoListaItemDTO';

import { useDownloader } from '@/hooks/downloader';
const toast = useToast();

// Components
import ListaPedidos from '@/features/pedidos-feature/pages/dashboard/components/ListaPedidos.vue';
import { RmCard, RmPaginator, RmText } from '@/components';
import { useFiltrosPadrao } from '@/hooks/filtros';
import pedidoService from '@/services/pedido-service';
import { useToast } from '@/hooks/toast';


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

const { baixarBase64 } = useDownloader();

const STATUS_PEDIDOS_ABERTOS = 'A';

const { filtros } = useFiltrosPadrao();
const pedidos = ref<PedidoListaItemDTO[]>([]);
const carregando = ref(true);
const comErro = ref(false);

/** Baixar nota fiscal */
const baixarNotaFiscal = async (dados: { idPedido: string; notaFiscal: string }) => {
    const { idPedido, notaFiscal } = dados;

    toast({ mensagem: 'Aguarde enquanto processamos a nota fiscal' });
    try {
        const [requisicao] = pedidoService.obterNotaFiscal(idPedido, notaFiscal);
        const resp = await requisicao;

        await baixarBase64({ base64: resp.data, nomeArquivo: `nf-${notaFiscal}.pdf`, tipoMime: 'application/pdf' });

    } catch (error) {
        toast({ mensagem: "Não foi possível baixar a NF, por favor, tente novamente" });
    }
};

/** Baixar boleto */
const baixarBoleto = async (dados: { idPedido: string; boleto: string }) => {
    const { idPedido, boleto } = dados;
    toast({ mensagem: 'Aguarde enquanto processamos o seu boleto' });
    try {
        const [requisicao] = pedidoService.obterBoleto(idPedido, boleto);
        const resp = await requisicao;

        await baixarBase64({ base64: resp.data, nomeArquivo: `boleto-${boleto}.pdf`, tipoMime: 'application/pdf' });

    } catch (error) {
        toast({ mensagem: "Não foi possível baixar o boleto, por favor, tente novamente" });
    }
};

async function obterPedidos() {
    try {
        carregando.value = true;
        const [requisicao] = pedidoService.obterLista({
            de: props.start,
            ate: props.end,
            pagina: filtros.pagina,
            tamanho: filtros.tamanho,
            status: STATUS_PEDIDOS_ABERTOS,
            ordenarPor: filtros.ordenarPor,
            direcao: filtros.direcao,
            idRepresentante: filtros.idRepresentante
        });
        const resposta = await requisicao;
        pedidos.value = resposta.dados;
        filtros.totalPaginas = Math.max(resposta.totalPaginas, 1);
    } catch (error) {
        comErro.value = false;
    }
    finally {
        carregando.value = false;
    }
}

watch(() => props, () => obterPedidos(), { deep: true });
onMounted(() => obterPedidos());
</script>

<template>
    <RmCard>
        <div class="flex items-center mb-4">
            <RmText type="headline-small" class="flex-1 ">
                Pedidos em aberto
            </RmText>
            <RmPaginator v-model:page="filtros.pagina"
                         :total-pages="filtros.totalPaginas"
                         @on:page-change="obterPedidos" />
        </div>
        <div class="overflow-x-auto">
            <ListaPedidos v-model:order-by="filtros.ordenarPor"
                        v-model:direction="filtros.direcao"
                        style="min-width: 900px"
                        :loading="carregando"
                        :items="pedidos"
                        @on:select-invoice="baixarNotaFiscal"
                        @on:select-slip="baixarBoleto"
                        @on:order-changed="obterPedidos" />
        </div>
    </RmCard>



</template>
