<script lang="ts" setup>
defineProps({
    label: {
        type: String,
    }
});
</script>

<template>
    <div>
        <label v-if="label" class="text-xs font-semibold block mb-1.5 dark:text-white">{{ label }}</label>
        <p class="py-3 text-sm px-3 flex items-center w-full text-gray-900 bg-neutral-300 dark:bg-gray-600  rounded-md min-h-[2.75rem]">
            <slot />
        </p>
    </div>
</template>