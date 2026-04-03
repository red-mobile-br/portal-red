<script lang="ts" setup>
import { PropType } from 'vue';
import product from '@/assets/img/product.jpg';

defineProps({
    item: {
        type: Object as PropType<{ urlImagem?: string; nome?: string; unidades?: number }>,
        required: true
    }
});
</script>

<template>
    <div class="flex items-center py-2 border-b border-gray-200 dark:border-gray-600 last:border-0 dont-break-page">
        <div class="w-12 h-12 rounded shadow-lg overflow-hidden">
            <img :src="item.urlImagem || product"
                 class="block w-full h-full object-contain">
        </div>
        <p class="flex-1 text-xs px-2">
            {{ item.nome }}
        </p>
        <p class="font-bold text-xs">
            {{ item.unidades }} un.
        </p>
    </div>
</template>