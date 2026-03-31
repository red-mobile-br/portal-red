<script lang="ts" setup>
import { PropType } from 'vue';

defineProps({
    type: {
        type: String as PropType<'horizontal' | 'vertical'>,
        default: 'horizontal'
    }
});
</script>

<template>
    <div class=" bg-gray-300 dark:bg-gray-800" :class="type == 'horizontal' ? 'h-px' : 'w-px'" />
</template>