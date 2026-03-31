<script lang="ts" setup>
import ArrowRightIcon from '@/icons/ArrowRightIcon.vue';
import TimesIcon from '@/icons/TimesIcon.vue';
import { computed } from 'vue';

const props = defineProps({
    label: {
        type: String,
    },
    disabled: {
        type: Boolean
    },
    allowEmpty: {
        type: Boolean,
        default: false
    },
    hasValue: {
        type: Boolean,
        default: false
    }
});

defineEmits<{
    (e: 'on:click'): void,
    (e: 'on:clear'): void
}>();

const showClearButton = computed(() => {
    return props.allowEmpty && props.hasValue && !props.disabled;
});
</script>

<template>
    <div>
        <label v-if="label" class="text-xs font-semibold block mb-1.5 dark:text-white">
            {{ label }}
        </label>

        <div :class="['text-sm px-3 flex items-center w-full text-gray-900 bg-neutral-300 dark:bg-gray-600 rounded-md min-h-[2.75rem]',
                      disabled ? 'transition-none scale-100 cursor-default' : '']">
            <p class="flex-1 text-left">
                <slot />
            </p>
            <div v-if="showClearButton"
                 class="cursor-pointer mr-2"
                 @click.stop="$emit('on:clear')">
                <TimesIcon class="fill-gray-500 dark:fill-gray-400 w-5" />
            </div>
            <div v-if="!disabled"
                 class="cursor-pointer"
                 @click="$emit('on:click')">
                <ArrowRightIcon class="fill-primary w-8" />
            </div>
        </div>
    </div>
</template>