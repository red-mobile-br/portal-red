<script lang="ts" setup>
defineProps({
    label: {
        type: String,
        required: true
    },
    selected: {
        type: Boolean,
        default: false
    }
});

defineEmits(['on:pressed']);
</script>

<template>
    <div class="rm-tab"
         :class="{'rm-tab_selected': selected}"
         @click="$emit('on:pressed')">
        {{ label }}
    </div>
</template>

<style>
.rm-tab {
    @apply text-neutral-900 dark:text-white opacity-40 border-b-2 border-transparent transition-all duration-300 py-2 inline-block align-top mr-7 cursor-pointer lg:hover:bg-gray-100 lg:dark:hover:bg-gray-700;
}
.rm-tab_selected {
    @apply !opacity-100 border-primary font-semibold;
}
</style>