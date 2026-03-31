<script lang="ts" setup>
import { computed, inject, onMounted, PropType, Ref } from 'vue';
import { FormField } from './RmForm.vue';

const props = defineProps({
    label: {
        type: String,
    },
    placeholder: {
        type: String,
        required: true
    },
    modelValue: {
        type: String,
        required: true
    },
    rules: {
        type: Array as PropType<((value: string) => string)[]>,
    },
    name: {
        type: String,
        required: true
    },
});

const emits = defineEmits(['update:modelValue']);

const addField = inject<(data: FormField) => null>("addField");
const fields = inject<Ref<FormField[]>>('fields');
const onInput = inject<() => void>('onInput');

const error = computed(() => fields?.value.find(el => el.name == props.name)?.error || "");

const inputValue = computed({
    get() {
        onInput && onInput();
        return props.modelValue;
    },
    set(value: any) {
        emits("update:modelValue", value);
    }
});

onMounted(() => {
    if(fields && props.rules && addField) {
        addField({ name: props.name, error: "", validation: props.rules });
    }
});
</script>


<template>
    <div class="w-full">
        
        <!-- Label -->
        <label v-if="label" class="text-sm font-semibold block mb-1.5 dark:text-white">{{ label }}</label>

        <!-- Input -->
        <textarea v-model="inputValue"
                  class="w-full relative h-24 rounded-md  dark:bg-gray-600 bg-neutral-300 flex items-center outline-none shadow-inset resize-none p-3 text-sm light-scroll"
                  :name="name"
                  :placeholder="placeholder" />

        <!-- Error -->
        <p v-if="error" class="text-xs font-medium mt-1.5 opacity-80">
            {{ error }}
        </p>
    </div>
</template>
