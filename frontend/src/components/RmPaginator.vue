<script lang="ts" setup>
import { nextTick } from 'vue';
import { AngleLeftIcon, AngleRightIcon } from '../icons';

const props = defineProps({
    page: {
        type: Number,
        required: true
    },
    totalPages: {
        type: Number,
        required: true
    }
});

const emits = defineEmits(['update:page', 'on:page-change']);

const changePage = (nextPage: number) => {
    const page = (props.page ?? 1) + nextPage;
    if(page > 0 && page <= (props.totalPages ?? 1)) {
        emits('update:page', page);
        nextTick(() => {
            emits('on:page-change', page);
        });
    }
};
        
</script>

<template>
    <div class="flex items-center">
        <button @click="changePage(-1)">
            <AngleLeftIcon class="w-6" :class="page == 1 ? 'fill-gray-300' : 'fill-primary'" />
        </button>
        <p class="text-xs font-bold mx-2">
            {{ page }}  |  {{ totalPages }}
        </p>
        <button @click="changePage(1)">
            <AngleRightIcon class="w-6" :class="page == totalPages ? 'fill-gray-300' : 'fill-primary'" />
        </button>
    </div>
</template>