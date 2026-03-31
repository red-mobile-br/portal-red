<script lang="ts" setup>
import { computed, PropType } from 'vue';
import RmDataTable, { DataTableColumns } from '@/components/RmDataTable.vue';
import ItemListaClientes from './ItemListaClientes.vue';

import ClienteListaItemDTO from '@/core/dtos/cliente/ClienteListaItemDTO';
import { useAutorizacao } from '@/hooks/autorizacao';

const props = defineProps({
    canOrder: {
        type: Boolean,
        default: true
    },
    loading: {
        type: Boolean,
        default: false
    },
    items: {
        type: Array as PropType<ClienteListaItemDTO[]>,
        required: true
    },
    orderBy: {
        type: String,
    },
    direction: {
        type: String,
    }
});
const emits = defineEmits(['update:orderBy', 'update:direction', 'on:select-customer']);
const { eGerente } = useAutorizacao();

const columns: DataTableColumns[] = [
    {
        title: 'Cód.',
        orderKey: 'id',
        size: 4
    },
    {
        title: 'Loja',
        size: 2.5
    },
    {
        title: 'Cliente',
        orderKey: 'nome',
    },
    {
        title: 'CNPJ',
        orderKey: 'cnpj',
        size: 8
    },
    {
        title: 'Cidade',
        orderKey: 'cidade',
        size: 8
    },
    {
        title: 'UF',
        orderKey: 'uf',
        size: 3
    },
    {
        title: 'Última compra',
        orderKey: 'ultimaCompra',
        size: 8
    },
    {
        title: 'Dias s/ comprar',
        orderKey: 'diasSemComprar',
        size: 8
    },
    {
        title: 'Status',
        orderKey: 'status',
        size: 6
    },
    {
        title: 'Representante',
        orderKey: 'idRepresentante',
        size: 8,
        hidden: !eGerente.value
    }
];

const handleOrderBy = computed({
    get: () => props.orderBy || '',
    set: (value: string) => emits('update:orderBy', value)
});

const handleDirection = computed({
    get: () => props.direction || '',
    set: (value: string) => emits('update:direction', value)
});

</script>

<template>
    <RmDataTable v-model:orderBy="handleOrderBy" 
                 v-model:direction="handleDirection"
                 empty-text="Nenhum cliente encontrado"
                 :columns="columns"
                 :can-order="canOrder"
                 :data="items"
                 :loading="loading">
        <template #default="{ data }">
            <ItemListaClientes :client="data" @click="$emit('on:select-customer', data.cnpj)" />
        </template>
    </RmDataTable>
</template>
