<script lang='ts' setup>
import { PropType } from 'vue';
import * as icons from '../icons';

defineProps({
    showIcon: {
        type: Boolean,
        default: true
    },
    icon: {
        type: String as PropType<icons.Icons>,
    },
    label: {
        type: String,
        required: true
    }
});


</script>

<template>
    <div class="min-w-[9rem] h-10 flex items-center px-2 cursor-pointer hover:bg-gray-100 dark:hover:bg-gray-700 rounded-md">
         
        <component :is="(icons as any)[icon]"
                   v-if="icon"
                   :class="{'!opacity-0': !showIcon}"
                   class="fill-current text-neutral-900 dark:text-white opacity-60 w-4"
        />
        <p class="whitespace-nowrap text-sm px-2 opacity-80">
            {{ label }}
        </p>
    </div>
</template>