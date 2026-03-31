<script lang="ts" setup>
import { onMounted, ref, watch } from 'vue';
import { Canceler } from 'axios';

import { useFiltrosPadrao } from '@/hooks/filtros';
import { useToast } from '@/hooks/toast';
import { useDownloader } from '@/hooks/downloader';

// Includes
import DialogoAcompanharPedido from '@/features/pedidos-feature/pages/dashboard/components/DialogoAcompanharPedido.vue';
import ListaPedidos from '@/features/pedidos-feature/pages/dashboard/components/ListaPedidos.vue';

// Customers
import { RmCard , RmText, RmPaginator } from '@/components';

// Services e DTOS
import pedidoService from '@/services/pedido-service';
import { clienteService } from '@/services/cliente-service';
import PedidoListaItemDTO from '@/core/dtos/pedido/PedidoListaItemDTO';

const props = defineProps({
    cnpj: {
        type: String,
        required: true
    }
});

const { filtros, aplicarFiltros } = useFiltrosPadrao();
const toast = useToast();
const { baixarBase64 } = useDownloader();

const carregando = ref(true);
const pedidos = ref<PedidoListaItemDTO[]>([]);
const pedidoSelecionado = ref<PedidoListaItemDTO>();
const comErro = ref(false);

let canceladorPedidos: Canceler | null;
async function obterPedidos(pagina: number) {

    try {
        canceladorPedidos && canceladorPedidos();
        carregando.value = true;
        aplicarFiltros();

        const [requisicao, cancelador] = clienteService.obterPedidosCliente(props.cnpj, {
            pagina: pagina,
            tamanho: filtros.tamanho,
            ordenarPor: filtros.ordenarPor,
            direcao: filtros.direcao,
            status: filtros.status,
            idRepresentante: filtros.idRepresentante
        });

        canceladorPedidos = cancelador;
        const resp = await requisicao;
        pedidos.value = resp.dados;
        filtros.totalPaginas = Math.max(resp.totalPaginas, 1);

    } catch (error) {
        comErro.value = true;
    }
    finally {
        carregando.value = false;
    }
}


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

watch(() => props.cnpj, () => {
    filtros.pagina = 1;
    obterPedidos(filtros.pagina);
});

onMounted(() => obterPedidos(filtros.pagina));

</script>

<template>
    <RmCard>
        <div class="flex items-center mb-4">
            <RmText type="headline-small" class="flex-1">
                Últimos pedidos
            </RmText>
            <RmPaginator v-model:page="filtros.pagina"
                         :total-pages="filtros.totalPaginas"
                         @on:page-change="obterPedidos" />

        </div>
        <p v-if="comErro" class="text-center text-sm">
            Não foi possível carregar os dados, tente novamente em breve
        </p>
        <ListaPedidos v-else
                    v-model:order-by="filtros.ordenarPor"
                    v-model:direction="filtros.direcao"
                    style="min-width: 900px"
                    :loading="carregando"
                    :items="pedidos"
                    @on:select-order="pedidoSelecionado = $event"
                    @on:select-invoice="baixarNotaFiscal"
                    @on:select-slip="baixarBoleto" />
    </RmCard>

    <DialogoAcompanharPedido v-model:order="pedidoSelecionado" />
</template>
