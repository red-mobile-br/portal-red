<script lang="ts" setup>
import { computed, inject, nextTick, onBeforeUnmount, onMounted, PropType, Ref } from 'vue';
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
});

const emits = defineEmits(['update:modelValue']);

const addField = inject<(data: FormField) => null>("addField");
const fields = inject<Ref<FormField[]>>('fields');
const onInput = inject<() => void>('onInput');
const removeField = inject<(name: string) => null>("removeField");

const error = computed(() => fields?.value.find(el => el.name == props.name)?.error || "");
const isRequired = computed(() => props.rules?.some(fn => (fn as any).__isRequired === true) ?? false);

const inputValue = computed({
    get() {
        nextTick(() => {
            onInput && onInput();
        });
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

onBeforeUnmount(() => removeField && removeField(props.name));
</script>

<template>
    <div class="w-full input" :data-field-name="name">
        
        <!-- Label -->
        <label v-if="label" class="text-sm font-semibold block mb-1.5 dark:text-white">
            {{ label }}<span v-if="isRequired" class="text-red-500 ml-0.5">*</span>
        </label>

        <div class="w-full relative h-11 rounded-md flex items-center pr-2 shadow-inset"
             :class="error ? 'bg-red-50 dark:bg-red-900/20 border border-red-400' : 'bg-neutral-300 dark:bg-gray-600'">

            <!-- Input -->
            <select v-model="inputValue"
                    class="flex-1 px-3 text-sm  dark:text-white h-11 bg-transparent outline-none min-w-0"
                    :class="{'opacity-50': modelValue.toString().length == 0}"
                    :name="name">
                <option hidden value="">
                    {{ placeholder }}
                </option>
                <slot />
            </select>
        </div>
        
        <!-- Error -->
        <p v-if="error" class="text-xs font-medium mt-1.5 text-red-500">
            {{ error }}
        </p>
    </div>
</template>
