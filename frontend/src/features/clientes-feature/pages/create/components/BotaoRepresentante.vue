<script lang="ts" setup>
import ArrowRightIcon from '@/icons/ArrowRightIcon.vue';

defineProps({
    label: {
        type: String,
    },
    disabled: {
        type: Boolean
    }
});

defineEmits<{
    (e: 'on:click'): void
}>();
</script>

<template>
    <div>
        <label v-if="label" class="text-xs font-semibold block mb-1.5 dark:text-white">
            {{ label }}
        </label>

        <button type="button" 
                :disabled="disabled"
                class="disabled:transition-none disabled:scale-100 disabled:cursor-default text-sm px-3 flex items-center w-full text-gray-900 bg-neutral-300 dark:bg-gray-600  rounded-md min-h-[2.75rem]"
                @click="$emit('on:click')">
            <p class="flex-1 text-left">
                <slot />
            </p>
            <ArrowRightIcon v-if="!disabled" class="fill-primary w-8" />
        </button>
    </div>
</template>