<script lang="ts" setup>
import { PropType, Ref, computed, inject, onMounted, onBeforeUnmount } from 'vue';
import { FormField } from './RmForm.vue';
import { vMaska } from 'maska';
import * as icons from '../icons';
import { Icons } from '@/icons';

const props = defineProps({
    showClearButton: {
        type: Boolean,
        default: false
    },
    type: {
        type: String,
    },
    label: {
        type: String,
    },
    placeholder: {
        type: String,
        required: true
    },
    modelValue: {
        type: [String, Number],
        required: true
    },
    rules: {
        type: Array as PropType<((value: string) => string)[]>,
    },
    name: {
        type: String,
        required: true
    },
    mask: {
        type: [String, Array] as PropType<string | string[]>
    },
    maskTokens: {
        type: String
    },
    showSubmit: {
        type: Boolean,
        default: false
    },
    submitIcon: {
        type: String as PropType<Icons>,
        default: 'ArrowRightIcon'
    },
    disabled: {
        type: Boolean,
        default: false
    }
});

const emit = defineEmits<{
    (e: 'update:modelValue', value: string): void,
    (e: 'on:clear'): void,
}>();

const addField = inject<(data: FormField) => null>("addField");
const removeField = inject<(name: string) => null>("removeField");
const fields = inject<Ref<FormField[]>>('fields');
const onInput = inject<() => void>('onInput');

const error = computed(() => fields?.value.find(el => el.name == props.name)?.error || "");
const isRequired = computed(() => props.rules?.some(fn => (fn as any).__isRequired === true) ?? false);

const inputValue = computed({
    get() {
        onInput && onInput();
        return props.modelValue;
    },
    set(value: any) {
        emit("update:modelValue", value);
    }
});

const submitIcon = computed(() => (icons as any)[props.submitIcon]);

onMounted(() => {
    if(fields && props.rules && addField) {
        addField({ name: props.name, error: "", validation: props.rules });
    }
});

onBeforeUnmount(() => removeField && removeField(props.name));
</script>

<template>
    <div :data-field-name="name" :class="{'opacity-60': disabled}">
        
        <!-- Label -->
        <label v-if="label"
               class="text-sm font-semibold block mb-1.5 dark:text-white">
            {{ label }}<span v-if="isRequired" class="text-red-500 ml-0.5">*</span>
        </label>

        <div class="w-full relative h-11 rounded-md flex items-center"
             :class="error ? 'bg-red-50 dark:bg-red-900/20 border border-red-400' : 'bg-neutral-300 dark:bg-gray-600'"
             style="box-shadow: inset 0 0 5px rgba(0, 0, 0, 0.1);">

            <!-- Input -->
            <input v-model="inputValue"
                   v-maska
                   class="flex-1 px-3 text-sm  rounded-md dark:text-white h-11 bg-transparent outline-none"
                   :data-maska="mask"
                   :data-maska-tokens="maskTokens"
                   :name="name"
                   :type="type"
                   :autocomplete="type == 'password' ? 'current-password' : ''"
                   :placeholder="placeholder"
                   :readonly="disabled">

            <!-- Submeter -->
            <div v-if="showClearButton"
                 class="cursor-pointer"
                 @click.prevent="$emit('on:clear')">
                <component :is="icons['TimesIcon']" 
                           class="fill-gray-300 w-6" />
            </div>

            <!-- Submeter -->
            <button v-if="showSubmit"
                    class="px-2"
                    :disabled="disabled">
                <component :is="submitIcon"
                           class="w-8 fill-primary" />
            </button>
        </div>
        
        <!-- Error -->
        <p v-if="error"
           class="text-xs text-left font-medium mt-1.5 text-red-500">
            {{ error }}
        </p>
    </div>
</template>