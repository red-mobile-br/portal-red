<script lang="ts" setup>
import { PropType, computed } from 'vue';
import { FormGroupStatus } from '../hooks/form-status';
import  * as icons from '@/icons';

const props = defineProps({
    status: {
        type: String as PropType<FormGroupStatus>,
        required: true
    },
});

const icon = computed(() => {
    if(props.status == 'invalid')  {
        return icons['TimesIcon'];
    }
    if(props.status == 'done') {
        return icons['CheckIcon'];
    }
    if(props.status == 'active') {
        return icons['EllipsisHorizontalIcon'];
    }
    return null;
});

</script>
<template>
    <div class="flex items-center gap-4 border-b border-gray-100 dark:border-gray-600 py-2">
        <div class="w-5 h-5 rounded-full border-2 flex items-center justify-center duration-150 transition-all"
             :class="{
                 'border-green-500 bg-green-500': status == 'done',
                 'border-red-500 bg-red-500': status == 'invalid',
                 'border-blue-500 bg-blue-500': status == 'active',
             }">
            <component :is="icon" v-if="icon" :key="status" class="w-4 h-4  fill-white" />
        </div>
        <p :class="{
            'text-gray-400 dark:text-gray-500': status === 'pending',
            'text-gray-600 dark:text-gray-400': status === 'active',
            'text-red-500 dark:text-red-400 font-medium': status === 'invalid',
            'text-green-600 dark:text-green-400': status === 'done',
        }">
            <slot />
        </p>
    </div>

</template>