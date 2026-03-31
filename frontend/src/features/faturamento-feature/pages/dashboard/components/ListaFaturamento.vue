<script lang="ts" setup>
import { computed, PropType } from 'vue';
import RmDataTable, { DataTableColumns } from '@/components/RmDataTable.vue';
import FaturamentoListaItemDTO from '@/core/dtos/faturamento/FaturamentoListaItemDTO';
import ItemListaFaturamento from './ItemListaFaturamento.vue';
import { useAutorizacao } from '@/hooks/autorizacao';

const props = defineProps({
    loading: {
        type: Boolean,
        default: false
    },
    items: {
        type: Array as PropType<FaturamentoListaItemDTO[]>,
        required: true
    },
    orderBy: {
        type: String,
        required: true 
    },
    direction: {
        type: String,
        required: true
    }
});

const emits = defineEmits(['update:orderBy', 'update:direction']);
const { eGerente } = useAutorizacao();
        
const columns: DataTableColumns[] = [
    {
        title: 'Pedido',
        orderKey: 'order',
        size: 5
    },
    {
        title: 'Cliente',
        orderKey: 'client'
    },
    {
        title: 'Data',
        orderKey: 'dateTime',
        size: 5
    },
    {
        title: 'NF',
        orderKey: 'receipt',
        size: 5
    },
    {
        title: 'Valor pedido + IPI',
        orderKey: 'baseValue',
        size: 10
    },
    {
        title: 'Valor NF',
        orderKey: 'orderValue',
        size: 8
    },
    {
        title: 'Representante',
        orderKey: 'vendor',
        size: 8,
        hidden: !eGerente.value
    },
];

const handleOrderBy = computed({
    get: () => props.orderBy,
    set: (value: string) => emits('update:orderBy', value)
});
        
const handleDirection = computed({
    get: () => props.direction,
    set: (value: string) => emits('update:direction', value)
});
</script>

<template>
    <RmDataTable v-model:orderBy="handleOrderBy" 
                 v-model:direction="handleDirection"
                 :columns="columns"
                 :data="items"
                 empty-text="Nenhum faturamento durante o período"
                 :loading="loading">
        <template #default="{ data }">
            <ItemListaFaturamento :revenue="data" />
        </template>
    </RmDataTable>
</template>