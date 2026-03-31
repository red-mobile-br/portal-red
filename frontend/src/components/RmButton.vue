<script lang="ts" setup>
defineProps({
    type: {
        type: String,
        default: 'primary'
    }
});
</script>

<template>
    <button :class="type">
        <slot />
    </button>
</template>