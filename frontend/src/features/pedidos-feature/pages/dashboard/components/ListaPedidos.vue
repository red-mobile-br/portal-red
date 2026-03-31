<script lang="ts" setup>
import { PropType, computed } from 'vue';

// Services and dtos
import PedidoListaItemDTO from '../../../../../core/dtos/pedido/PedidoListaItemDTO';

// Hooks
import { useAutorizacao } from '../../../../../hooks/autorizacao';

// Components
import RmDataTable, { DataTableColumns } from '../../../../../components/RmDataTable.vue';
import ItemListaPedidos from './ItemListaPedidos.vue';

const props = defineProps({
    loading: {
        type: Boolean,
        default: false
    },
    items: {
        type: Array as PropType<PedidoListaItemDTO[]>,
        required: true
    },
    orderBy: {
        type: String,
    },
    direction: {
        type: String,
    },
    isQuotePage: {
        type: Boolean,
    }
});

const title = props.isQuotePage ? "Orçamento" : "Pedido";

const emits = defineEmits(['update:orderBy', 'update:direction', 'on:select-order', 'on:select-invoice', 'on:select-slip']);

const { eGerente } = useAutorizacao();

const columns: DataTableColumns[] = [
    {
        title: title,
        orderKey: 'id',
        size: 4.5
    },
    {
        title: 'Cód. cliente',
        orderKey: 'idCliente',
        size: 7
    },
    {
        title: 'Loja',
        size: 3.5,
    },
    {
        title: 'Cliente',
        orderKey: 'nome'
    },
    {
        title: 'CNPJ',
        orderKey: 'cnpj',
        size: 8
    },
    {
        title: 'Emissão',
        orderKey: 'dataLancamento',
        size: 5
    },
    {
        title: 'Valor total',
        orderKey: 'valorTotal',
        size: 6
    },
    {
        title: 'Representante',
        orderKey: 'representante',
        size: 8,
        hidden: !eGerente.value
    },
    {
        title: 'Status',
        orderKey: 'status',
        size: 5
    },
    {
        title: "",
        size: 2.5
    }
];

const handleOrderBy = computed({
    get: () => props.orderBy,
    set: (value?: string) => emits('update:orderBy', value)
});

const handleDirection = computed({
    get: () => props.direction,
    set: (value?: string) => emits('update:direction', value)
});
</script>

<template>
    <RmDataTable v-model:orderBy="handleOrderBy" 
                 v-model:direction="handleDirection"
                 :empty-text="`Nenhum ${title.toLowerCase()} durante o período`"
                 :columns="columns"
                 :data="items"
                 :loading="loading">

        <template #default="{ data, index }">
            <ItemListaPedidos :order="data" :is-last="index > 8"
                            @click="$emit('on:select-order', data)"
                            @on:select-invoice="$emit('on:select-invoice', $event)"
                            @on:select-slip="$emit('on:select-slip', $event)" />
        </template>
        
    </RmDataTable>
</template>