<script lang="ts" setup>
import { PropType, computed } from 'vue';
import * as icons from '../icons';
import { Icons } from '../icons';

const props = defineProps({
    icon: {
        type: String as PropType<Icons>,
        required: true
    },
    iconClass: {
        type: String,
        defalt: 'fill-primary w-6'
    },
});

const iconComponent = computed(() => (icons as any)[props.icon]);

</script>

<template>
    <button class="rounded-full active:bg-white active:bg-opacity-30 flex items-center justify-center 2.5rem">
        <component :is="iconComponent"
                   :class="iconClass" />
    </button>
</template>