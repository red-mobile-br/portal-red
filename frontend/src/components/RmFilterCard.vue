<script lang="ts" setup>
defineProps({
    trimBottom: {
        type: Boolean,
        default: false
    }
});
</script>

<template>
    <div class="bg-white dark:bg-gray-700 p-5 rounded-xl shadow-main" :class="{'rounded-b-none': trimBottom}">
        <slot />
    </div>
</template>
