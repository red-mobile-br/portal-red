<script lang="ts" setup>
import { computed, inject, onMounted, PropType, Ref } from 'vue';
import { vMaska } from 'maska';
import { FormField } from './RmForm.vue';
import { useDatePicker } from '../hooks/date-picker';
import { dataValida } from '../utils/validadores';
import { parse, format } from 'date-fns';
import { CalendarIcon } from '@/icons';

const props = defineProps({
    label: {
        type: String,
    },
    submitLabel: {
        type: String,
        default: "Filtrar"
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
    disabled: {
        type: Boolean,
        default: false
    },
    canSelectFutureDates: {
        type: Boolean,
        default: true
    }
});

const emits = defineEmits(['update:modelValue']);

const { selecionarData: selectDate } = useDatePicker();

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

const openDatePicker = async () => {

    const initialDate = dataValida(props.modelValue.toString()).length == 0
        ?  parse(props.modelValue.toString(), 'dd/MM/yyyy', new Date()) 
        : new Date();

    const newDate = await selectDate({
        initialDate,
        submitLabel: props.submitLabel,
        canSelectFutureDates: props.canSelectFutureDates
    });

    if(newDate != null) {
        emits('update:modelValue', format(newDate, 'dd/MM/yyyy'));
    }
};

onMounted(() => {
    if(fields && props.rules && addField) {
        addField({ name: props.name, error: "", validation: props.rules });
    }
});
</script>

<template>
    <div :class="{'opacity-60': disabled}">
        
        <!-- Label -->
        <label v-if="label" class="text-sm font-semibold block mb-1.5 dark:text-white">{{ label }}</label>

        <div class="w-full relative h-11 rounded-md  dark:bg-gray-600 bg-neutral-300 flex items-center"
             style="box-shadow: inset 0px 0px 5px rgba(0, 0, 0, 0.1);">

            <!-- Input -->
            <input v-model="inputValue"
                   v-maska
                   data-maska="##/##/####"
                   class="flex-1 px-3 text-sm  rounded-md dark:text-white h-11 bg-transparent outline-none"
                   :name="name"
                   :placeholder="placeholder"
                   :readonly="disabled">

            <!-- Submeter -->
            <button class="px-2" @click.prevent="openDatePicker">
                <CalendarIcon class="fill-primary w-5" />
            </button>
        </div>
        
        <!-- Error -->
        <p v-if="error" class="text-xs text-left font-medium mt-1.5 opacity-80 text-red-500">
            {{ error }}
        </p>
    </div>
</template>