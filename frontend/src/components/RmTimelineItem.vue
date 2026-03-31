<script lang="ts" setup>
import { formatarDataHora } from '../utils/string-functions';
defineProps({
    date: {
        type: String,
        required: true
    },
    title: {
        type: String,
        required: true
    },
    subtitle: {
        type: String,
    }
});
</script>

<template>
    <div class="w-full flex items-center">
        <p class="text-sm font-bold">
            {{ formatarDataHora(date) }}
        </p>

        <div class="w-10 flex items-center justify-center relative self-stretch">
            <div class="w-1.5 h-1.5 rounded-full bg-primary" />
            <div class="w-0.5 top-0 absolute bg-primary bottom-0" />
        </div>

        <div class="flex-1 text-left py-2">
            <p class="text-sm">
                {{ title }}
            </p>
            <p v-if="subtitle" class="text-xs opacity-70">
                {{ subtitle }}
            </p>
        </div>
        <slot />
    </div>
</template>