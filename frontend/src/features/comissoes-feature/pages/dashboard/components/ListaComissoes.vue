<script lang="ts" setup>
import { computed, PropType } from 'vue';
import RmDataTable, { DataTableColumns } from '@/components/RmDataTable.vue';
import ItemListaComissoes from './ItemListaComissoes.vue';
import { useAutorizacao } from '@/hooks/autorizacao';
import ComissaoListaItemDTO from '@/core/dtos/comissoes/ComissaoListaItemDTO';

const props = defineProps({
    loading: {
        type: Boolean,
        default: false
    },
    items: {
        type: Array as PropType<ComissaoListaItemDTO[]>,
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
const emits = defineEmits(['update:orderBy','update:direction' ]);
const { eGerente } = useAutorizacao();
        
const columns: DataTableColumns[] = [
    {
        title: 'Pedido',
        orderKey: 'order',
        size: 4.5
    },
    {
        title: 'Cliente',
        orderKey: 'clientName'
    },
    {
        title: 'Titulo',
        orderKey: 'title',
        size: 4.5
    },
    {
        title: 'Parcela',
        orderKey: 'installment',
        size: 4.5
    },
    {
        title: 'Venc. original',
        orderKey: 'dueDate',
        size: 7
    },
    {
        title: 'Data baixa',
        orderKey: 'writeOffDate',
        size: 6
    },
    {
        title: 'Valor título',
        orderKey: 'titleValue',
        size: 6
    },
    {
        title: 'Base de cálculo',
        orderKey: 'baseValue',
        size: 8
    },
    {
        title: '%',
        orderKey: 'commissionPercentage',
        size: 2.5
    },
    {
        title: 'Representante',
        orderKey: 'vendor',
        size: 8,
        hidden: !eGerente.value
    },
    {
        title: 'Valor comissão',
        orderKey: 'commissionValue',
        size: 8
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
                 empty-text="Nenhuma comissão para os filtros informados"
                 :loading="loading">
        <template #default="{ data }">
            <ItemListaComissoes :commission="data" />
        </template>
    </RmDataTable>
</template>
