<script lang="ts" setup>
import { PropType, computed } from 'vue';
import AngleDownIcon from '../icons/AngleDownIcon.vue';
import RmTableSkeleton from './RmTableSkeleton.vue';

export interface DataTableColumns {
    title: string;
    size?: number;
    orderKey?: string;
    hidden?: boolean;
}


const props = defineProps({
    canOrder: {
        type: Boolean,
        default: true
    },
    orderBy: {
        type: String,
        default: ""
    },
    direction: {
        type: String,
        default: ""
    },
    loading: {
        type: Boolean,
        default: false
    },
    emptyText: {
        type: String
    },
    columns: {
        type: Array as PropType<DataTableColumns[]>,
        required: true
    },
    data:{
        type: Array as PropType<any[]>,
        required: true
    }
});

const emits = defineEmits(['update:direction', 'update:orderBy', 'on:order-changed']);

const visibleColumns = computed(() => props.columns.filter(el => !el.hidden));

const selectColumn = (title: string) => {
    emits('update:direction', props.orderBy != title || (props.orderBy == title && props.direction == 'desc') ? 'asc' : 'desc');
    emits('update:orderBy', title);
    emits('on:order-changed');
};
</script>

<template>
    <table class="red-mobile-table">
        <!-- Header -->
        <tr>
            <th v-for="column in visibleColumns" :key="column.title" 
                :class="{'cursor-pointer hover:underline': column.orderKey && canOrder }"
                :style="{'width': `${column.size}rem`, 'text-align': column.size ? 'center' : 'left'}"
                @click="column.orderKey && canOrder ? selectColumn(column.orderKey) : null">

                <span>{{ column.title }}</span>
                <AngleDownIcon v-if="column.orderKey && orderBy == column.orderKey"
                               class="w-4 fill-gray-900 inline align-top"
                               :class="{'rotate-180': direction == 'desc'}" />
            </th>
        </tr>
        <template v-if="!loading">
            <slot v-for="(column, index) in data" :key="index" :data="column" :index="index" />
        </template>

        <!-- Loading -->
        <RmTableSkeleton v-for="n in loading ? 12 : 0" :key="n" :columns="visibleColumns.length" />

        <!-- Vazio -->
        <tr v-if="!data.length && !loading">
            <td :colspan="visibleColumns.length">
                {{ emptyText }}
            </td>
        </tr>
    </table>
</template>