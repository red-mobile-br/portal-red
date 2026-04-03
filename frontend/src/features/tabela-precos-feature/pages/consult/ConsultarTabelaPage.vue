<script lang="ts" setup>
import { reactive, watch } from 'vue';
import { useRouter } from 'vue-router';

import TabelaPrecosDTO from '@/core/dtos/tabelasPrecos/TabelaPrecosDTO';
import tabelaPrecosService from '@/services/tabela-precos-service';

import { 
    RmCard, 
    RmText,
    RmDivider,
    RmTextField,
    RmLoading,
    RmSelect, 
    RmForm,
    RmButton 
} from '@/components';
import ItemListaTabelaPrecos from './components/ItemListaTabelaPrecos.vue';

import { formatarData } from '@/utils/string-functions';

interface ConsultTableViewState {
    loading: boolean;
    priceTable: TabelaPrecosDTO | null;
    selectedState: string;
}

const state = reactive<ConsultTableViewState>({
    loading: false,
    priceTable: null,
    selectedState: ''
});

const router = useRouter();

const tables = [
    { name: "Paraíba", value: "PB" },
    { name: "Brasil", value: "BR" }
];


const loadData = async () => {
    try {
        state.loading = true;
        const [request] = tabelaPrecosService.obterTabelaPrecos(state.selectedState);
        state.priceTable = await request;
                
    } catch (error) {
        window.alert("Não foi possível realizar a requisição, tente novamente em breve");  
    }
    finally {
        state.loading = false;
    }
};

const print = () => {
    const route = router.resolve({ name: 'impressaoTabela', query: { state: state.selectedState } });
    window.open(route.href, '_blank');
};

watch(() => state.selectedState, () => {
    state.priceTable = null;
    loadData();
});

</script>

<template>
    <div class="flex-1 flex flex-col lg:flex-row items-stretch gap-4">
        

        <RmCard class="flex-1 overflow-hidden flex flex-col">
            <RmText type="headline-small" class="mb-2">
                Selecione uma tabela:
            </RmText>
            <RmForm>
                <RmSelect v-model="state.selectedState" name="estado" class="max-w-xs" placeholder="Estado">
                    <option v-for="(table, index) in tables" :key="index" :value="table.value">
                        {{ table.name }}
                    </option>
                </RmSelect>
            </RmForm>

            <RmDivider class="my-4" />

            <transition name="fade" mode="out-in">
                <div v-if="state.loading" key="0" class="flex items-center justify-center flex-1">
                    <RmLoading />
                </div>

                <div v-else-if="state.priceTable" key="1">
                    <RmText type="headline-small" class="mb-4">
                        {{ state.priceTable.descricao }}
                    </RmText>

                    <div class="overflow-auto light-scroll w-full">
                        <table class="red-mobile-table --small" style="min-width: 650px">
                            <tr>
                                <th class="w-20">
                                    Cód.
                                </th>
                                <th class="!text-left">
                                    Produto
                                </th>
                                <th class="w-24">
                                    Preço s/ IPI
                                </th>
                                <th class="w-24">
                                    IPI (%)
                                </th>
                            </tr>
                            <ItemListaTabelaPrecos v-for="(product, index) in state.priceTable.produtos"
                                                :key="index"
                                                :product="product" />
                        </table>
                    </div>
                </div>
            </transition>
        </RmCard>


        <div class="w-full lg:w-80 mb-4 flex-shrink-0">
            <transition name="fade">
                <RmCard v-if="state.priceTable" class="!p-0 sticky top-4 flex flex-col items-stretch min-h-full">
                    <div class="flex-1 overflow-auto p-4">
                        <RmText type="headline-small" class="mb-4">
                            Informações gerais
                        </RmText>
                        <RmTextField class="mb-2" label="Últimas atualizações">
                            {{ formatarData(state.priceTable.dataAtualizacao ?? '') }}
                        </RmTextField>
                        <RmTextField class="mb-2" label="Prazo">
                            {{ formatarData(state.priceTable.prazo ?? '') }}
                        </RmTextField>
                        <RmTextField class="mb-2" label="Frete CIF">
                            A partir de  R$ {{ (state.priceTable.valorMinimoFreteCif ?? 0).toFixed(2) }}
                        </RmTextField>
                        <RmTextField class="mb-10" label="ICMS">
                            {{ state.priceTable.icms }}%
                        </RmTextField>

                        <p class="text-xs font-semibold">
                            {{ state.priceTable.comentarios }}
                        </p>
                        <p class="text-xs font-semibold">
                            Obs.: Preços referentes a comissão de 5%
                        </p>
                    </div>
                    <RmDivider />
                    <div class="p-4">
                        <RmButton @click="print">
                            Imprimir
                        </RmButton>
                    </div>
                </RmCard>
            </transition>
        </div>
    </div>
</template>