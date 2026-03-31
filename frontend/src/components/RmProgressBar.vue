<script lang="ts" setup>
defineProps({
    percentage: {
        type: Number,
        default: 0
    }
});
</script>

<template>
    <div class="w-full bg-gray-300 h-1 rounded-full">
        <div class="bg-primary rounded-full h-1" :style="{width: `${percentage}%`}" />
    </div>
</template>
