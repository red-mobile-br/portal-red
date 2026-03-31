<script lang="ts" setup>
import { computed, ref, nextTick } from 'vue';
import { RmDivider, RmDropdown, RmTextButton } from '.';

import RmFilterTag from './RmFilterTag.vue';
import RmIconButton from './RmIconButton.vue';
import isSameMonth from 'date-fns/isSameMonth';

const props = defineProps({
    modelValue: {
        type: Date,
        required: true
    }
});

const emits = defineEmits<{
    (e: 'update:modelValue', date: Date): void;
    (e:'on:date-change'): void;
}>();

const selectedMonth = computed(() => props.modelValue.getMonth());
const selectedYear = computed(() => props.modelValue.getFullYear());

const dateTemp = ref(props.modelValue);

const handleMonth = computed({
    get: () => dateTemp.value.getMonth(),
    set: (value: number) => dateTemp.value = new Date(dateTemp.value.getFullYear(), value)
});

const handleYear = computed({
    get: () => dateTemp.value.getFullYear(),
    set: (value: number) => dateTemp.value = new Date(value, dateTemp.value.getMonth())
});

const reset = () => {
    handleYear.value = new Date().getFullYear();
    handleMonth.value = new Date().getMonth();
};

const selectDate = () => {
    emits('update:modelValue', dateTemp.value);
    nextTick(() => emits('on:date-change'));
};

const clear = ()  => {
    dateTemp.value = new Date();
    selectDate();
};

const showMonthClearButton = computed(() => !isSameMonth(props.modelValue, new Date()));


const meses = ['Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho', 'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'];

</script>
<template>
    <RmDropdown position="bottom-left" :close-on-click="false">
        <RmFilterTag :show-clear-button="showMonthClearButton" @on:clear="clear">
            {{ meses[selectedMonth] }} - {{ handleYear }}
        </RmFilterTag>

        <template #content="{toogle}">
            <div class="w-52 h-64 flex flex-col items-stretch">
                <div class="flex items-center px-6 py-2">
                    <RmIconButton icon="AngleLeftIcon" 
                                  icon-class="fill-gray-800 dark:fill-white w-8" 
                                  @click="handleYear -= 1" />

                    <p class="flex-1 text-center font-semibold">
                        {{ handleYear }}
                    </p>
                    <RmIconButton icon="AngleRightIcon" 
                                  icon-class="fill-gray-800 dark:fill-white w-8" 
                                  @click="handleYear += 1" />
                </div>
                <RmDivider />
                
                <div class="grid grid-cols-3 flex-1 items-">
                    <div v-for="(month, index) in meses" :key="month" 
                         class="cursor-pointer hover:bg-black/5 dark:hover:bg-white/5 flex items-center justify-center"
                         :class="{
                             'bg-primary-light': selectedMonth == index && selectedYear == handleYear,
                             '!bg-primary': handleMonth == index
                         }"
                         @click="handleMonth = index">
                        <p>{{ month.substring(0, 3) }}</p>
                    </div>
                </div>
                
                <RmDivider />
                <div class="p-2 flex items-center">
                    <RmTextButton @click="reset">
                        Mês atual
                    </RmTextButton>
                    <div class="flex-1" />
                    <RmTextButton @click="() => {
                        selectDate();
                        toogle();
                    }">
                        Aplicar
                    </RmTextButton>
                </div>
                
            </div>
        </template>
    </RmDropdown>

</template>