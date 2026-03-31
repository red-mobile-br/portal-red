<script lang="ts" setup>
import { computed, nextTick, PropType } from 'vue';
import RmDropdown from './RmDropdown.vue';
import RmDropdownItem from './RmDropdownItem.vue';
import { RmFilterTag } from '.';

const props = defineProps({
    options: {
        type: Array as PropType<{name: string, value: any}[]>,
        required: true
    },
    modelValue: {
        required: true,
    },
    placeholder: {
        type: String,
        required: true
    },
    showClearButton: {
        type: Boolean,
        default: false
    }
});

const emits = defineEmits(['update:modelValue', 'on:change', 'on:clear']);

const selectedOption = computed(() => props.options.find(el => el.value == props.modelValue));

const selectOption = (option: any) => {
    const value = option == props.modelValue ? null : option;
    emits('update:modelValue', value);
    nextTick(() => {
        emits('on:change');
    });
};
</script>

<template>
    <RmDropdown position="bottom-left">
        <template #default>
            <RmFilterTag :show-clear-button="showClearButton"
                         @on:clear="$emit('on:clear')">
                {{ selectedOption?.name || placeholder }}
            </RmFilterTag>
        </template>
        <template #content>

            <RmDropdownItem v-for="option in options" 
                            :key="option.value" 
                            :label="option.name" 
                            icon="CheckIcon"
                            :show-icon="modelValue == option.value" 
                            @click="selectOption(option.value)" />
        </template>
    </RmDropdown>
</template>