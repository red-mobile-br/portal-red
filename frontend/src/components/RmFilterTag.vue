<script lang="ts" setup>
import TimesIcon from '../icons/TimesIcon.vue';
import AngleDownIcon from '../icons/AngleDownIcon.vue';
import RmFilterCard from './RmFilterCard.vue';
import RmText from './RmText.vue';

defineProps({
    hideArrow: {
        type: Boolean,
        default: false
    },
    showClearButton: {
        type: Boolean,
        default: false
    }
});

defineEmits(['on:clear']);

</script>
<template>
    <RmFilterCard class="py-2 px-3 flex items-center mr-2 cursor-pointer mb-2">
        <button v-if="showClearButton" class="pr-2" @click.stop="$emit('on:clear')">
            <TimesIcon icon="times" size="1rem" class="w-4 fill-red-500" />
        </button>
        <RmText type="body-small">
            <slot />
        </RmText>
        <AngleDownIcon v-if="!hideArrow" class="w-3 ml-1 fill-gray-800 dark:fill-white" />
    </RmFilterCard>
</template>

