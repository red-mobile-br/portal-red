<script lang="ts" setup>
import { PropType } from 'vue';

type TextTypes = 
    'display-large' |
    'display-medium' |
    'display-small' |
    'headline-medium' |
    'headline-small' |
    'body-medium' |
    'body-small' |
    'label-medium' |
    'label-small';

defineProps({
    type: {
        type: String as PropType<TextTypes> ,
        default: 'body-medium'
    }
});
</script>

<template>
    <p :class="type">
        <slot />
    </p>
</template>

<style>
    /* Textos */
    .display-large {
        @apply text-2xl font-semibold text-primary;
    }

    .display-medium {
        @apply text-2xl font-light dark:text-white opacity-70;
    }

    .display-small {
        @apply text-2xl font-bold dark:text-white;
    }

    .headline-medium {
        @apply opacity-80 dark:text-white;
    }

    .headline-small {
        @apply text-sm font-bold;
    }

    .body-medium {
        @apply text-neutral-900 dark:text-white;
    }

    .body-small {
        @apply text-xs font-semibold;
    }

    .label-medium {
        @apply dark:text-white;
        font-size: 10px;
    }

    .label-small {
        @apply opacity-50;
        font-size: 11px;
    }
</style>