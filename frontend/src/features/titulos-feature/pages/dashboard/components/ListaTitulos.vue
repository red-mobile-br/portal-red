<script lang="ts" setup>
import { computed, PropType } from 'vue';
import DataTable, { DataTableColumns } from '@/components/RmDataTable.vue';
import TituloListaItemDTO from '@/core/dtos/titulo/TituloListaItemDTO';
import ItemListaTitulos from './ItemListaTitulos.vue';
import { useAutorizacao } from '@/hooks/autorizacao';

const props = defineProps({
    loading: {
        type: Boolean,
        default: false
    },
    items: {
        type: Array as PropType<TituloListaItemDTO[]>,
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
        title: 'Título',
        orderKey: 'title',
        size: 5
    },
    {
        title: 'Parcela',
        orderKey: 'installment',
        size: 5,
    },
    {
        title: 'Pedido',
        orderKey: 'order',
        size: 5
    },
    {
        title: 'Cliente',
        orderKey: 'clientName',
    },
    {
        title: 'CNPJ',
        orderKey: 'cnpj',
        size: 8
    },
    {
        title: 'Venc. original',
        orderKey: 'originalDue',
        size: 7
    },
    {
        title: 'Venc. real',
        orderKey: 'dueDate',
        size: 7
    },
    {
        title: 'D. baixa',
        orderKey: 'paymentDate',
        size: 6
    },
    {
        title: 'Valor título',
        orderKey: 'titleValue',
        size: 7
    },
    {
        title: 'Representante',
        orderKey: 'vendor',
        size: 8,
        hidden: !eGerente.value
    },
    {
        title: 'Status',
        orderKey: 'status',
        size: 5
    }
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
    <DataTable v-model:orderBy="handleOrderBy" 
               v-model:direction="handleDirection"
               :columns="columns"
               :data="items"
               empty-text="Nenhum titulo durante o período"
               :loading="loading">
        <template #default="{ data }">
            <ItemListaTitulos :title="data" />
        </template>
    </DataTable>
</template>
