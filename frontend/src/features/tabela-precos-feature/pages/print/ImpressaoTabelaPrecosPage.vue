<script lang="ts" setup>
import { computed, onMounted, reactive } from 'vue';
import { useRoute } from 'vue-router';
import {
    RmPrintView,
    RmPrintPage,
    RmText, 
    RmDivider, 
    RmTextField,
    RmLoading 
} from '@/components';

import useAppStore from '@/store/app-store';
import { formatarData } from '@/utils/string-functions';
import TabelaPrecosDTO from '@/core/dtos/tabelasPrecos/TabelaPrecosDTO';
import tabelaPrecosService from '@/services/tabela-precos-service';
import ItemListaTabelaPrecos from '../consult/components/ItemListaTabelaPrecos.vue';

interface OrdersDashboardPrintViewState {
    priceTable: TabelaPrecosDTO | null;
    loading: boolean;
}

const { query } = useRoute();
const { nomeUsuario: name } = useAppStore();
const tableState = query['state'] ? query['state'].toString() : 'BR';

const state = reactive<OrdersDashboardPrintViewState>({
    loading: false,
    priceTable: null,
});

const nomeUsuario = computed(() => name.value);

const loadData = async () => {
    try {
        state.loading = true;
        const [request] = tabelaPrecosService.obterTabelaPrecos(tableState);
        state.priceTable = await request;

    } catch (error) {
        window.alert("Não foi possivel carregar os dados, por favor, tente novamente");
    }
    finally {
        state.loading = false;
    }
};

onMounted(() => loadData());
</script>

<template>
    <RmPrintView>
        <Transition name="fade" mode="out-in">
            <div v-if="state.loading" class="h-screen flex items-center justify-center">
                <RmLoading />
            </div>

            <RmPrintPage v-else-if="state.priceTable" title="Relatório de tabela de preços">
                <div class="w-full">

                    <!-- Header -->
                    <div class="flex items-stretch">
                        <RmText type="display-medium">
                            {{ state.priceTable.descricao }}
                        </RmText>
                    </div>

                    <Divider class="my-5" />
                    <RmText type="headline-medium" class="mb-4">
                        Representante
                    </RmText>
                    <RmTextField>{{ nomeUsuario }}</RmTextField>
                    <RmDivider class="my-5" />
                    <table class="red-mobile-table --ultra-small">
                        <tr>
                            <th class="w-16">
                                Cód.
                            </th>
                            <th class="!text-left">
                                Produto
                            </th>
                            <th class="w-20">
                                Preço s/ IPI
                            </th>
                            <th class="w-20">
                                IPI (%)
                            </th>
                        </tr>
                        <ItemListaTabelaPrecos v-for="(product, index) in state.priceTable.produtos"
                                            :key="index"
                                            :product="product" />
                    </table>
                    <RmDivider class="my-5" />
                    <RmText type="headline-medium" class="mb-4">
                        Informações gerais
                    </RmText>
                    <RmTextField class="mb-2" label="Últimas atualizações">
                        {{ formatarData(state.priceTable.dataAtualizacao ?? '') }}
                    </RmTextField>
                    <RmTextField class="mb-2" label="Validade">
                        {{ formatarData(state.priceTable.dataValidade ?? '') }}
                    </RmTextField>
                    <RmTextField class="mb-2" label="Frete CIF">
                        A partir de  R$ {{ (state.priceTable.valorMinimoFreteCif ?? 0).toFixed(2) }}
                    </RmTextField>
                    <RmTextField class="mb-10" label="ICMS">
                        {{ state.priceTable.icms }}%
                    </RmTextField>

                    <p class="text-xs font-semibold">
                        {{ state.priceTable.observacoes }}
                    </p>
                    <p class="text-xs font-semibold">
                        Obs.: Preços referentes a comissão de 5%
                    </p>
                </div>
            </RmPrintPage>
        </Transition>
    </RmPrintView>
</template>