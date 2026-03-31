<script lang="ts" setup>
import { computed, ref, PropType, nextTick } from 'vue';
import min from 'date-fns/min';
import max from 'date-fns/max';
import isSameMonth from 'date-fns/isSameMonth';
import lastDayOfMonth from 'date-fns/lastDayOfMonth';

import { RmDivider, RmDropdown, RmTextButton, RmFilterTag, RmIconButton } from '@/components';
import RmMonthRangePickerDate from '@/components/RmMonthRangePickerDate.vue';

import { meses } from '@/utils/meses';

const props = defineProps({
    modelValue: {
        type: Object as PropType<{ start: Date; end: Date }>,
        required: true
    }
});

const emits = defineEmits<{
    (e: 'update:modelValue', date: { start: Date; end: Date }): void;
    (e:'on:date-change'): void;
}>();

const hoverDate = ref<Date | null>(null);
const date1 = ref<Date | null>(props.modelValue.start);
const date2 = ref<Date | null>(props.modelValue.end);

const selectedYear = ref(props.modelValue.end.getFullYear());

const selectedStartMonth = computed(() => props.modelValue.start.getMonth());
const selectedStartYear = computed(() => props.modelValue.start.getFullYear());
const selectedEndMonth = computed(() => props.modelValue.end.getMonth());
const selectedEndYear = computed(() => props.modelValue.end.getFullYear());


const showMonthClearButton = computed(() => !isSameMonth(props.modelValue.start, new Date()) || !isSameMonth(props.modelValue.end, new Date()) );

const tagLabel = computed(() => {
    if(!showMonthClearButton.value) {
        return `${meses[selectedStartMonth.value]} - ${selectedStartYear.value}`;
    }
    return `${meses[selectedStartMonth.value]} de ${selectedStartYear.value} à ${meses[selectedEndMonth.value]} de ${selectedEndYear.value}`;
});

function clear() {
    date1.value = new Date();
    date2.value = new Date();
    emits('update:modelValue', { start: new Date(), end: new Date() });
}

function onSelecteDate(date: Date) {
    if(!date1.value) {
        date1.value = date;
        return;
    }

    if(!date2.value) {
        date2.value = date;
        return;
    }

    date1.value = date;
    date2.value = null;
}

function reset() {
    date1.value = props.modelValue.start;
    date2.value = props.modelValue.end;
}

function thisMonth() {
    date1.value = new Date();
    date2.value = new Date();
}

function apply() {
    if(!date1.value || !date2.value) return;
    const minDate = min([date1.value, date2.value]);
    const maxDate = max([date1.value, date2.value]);
    emits('update:modelValue', { start: minDate, end: lastDayOfMonth(maxDate) });
    nextTick(() => emits('on:date-change'));
}

function onCloseDialog() {
    if(!date1.value || !date2.value) {
        reset();
    }
}
</script>

<template>
    <RmDropdown position="bottom-left"
                :close-on-click="false" 
                @close="onCloseDialog">
        <RmFilterTag :show-clear-button="showMonthClearButton" @on:clear="clear">
            {{ tagLabel }}
        </RmFilterTag>

        <template #content="{ toogle }">
            <div class="w-52">
                <div class="flex items-center px-6 py-2">
                    <RmIconButton icon="AngleLeftIcon" 
                                  icon-class="fill-gray-800 dark:fill-white w-8" 
                                  @click="selectedYear -= 1" />

                    <p class="flex-1 text-center font-semibold text-gray-800 dark:text-white">
                        {{ selectedYear }}
                    </p>
                    <RmIconButton icon="AngleRightIcon" 
                                  icon-class="fill-gray-800 dark:fill-white w-8" 
                                  @click="selectedYear += 1" />
                </div>
                <RmDivider />
                
                <div class="grid grid-cols-3 gap-y-1 p-2 h-40">
                    <RmMonthRangePickerDate v-for="(month, index) in meses" :key="month" 
                                            :date="new Date(selectedYear, index)"
                                            :selected-date1="date1"
                                            :selected-date2="date2"
                                            :hover-date="hoverDate"
                                            @click="onSelecteDate(new Date(selectedYear, index))"
                                            @mouseover="hoverDate = new Date(selectedYear, index)" />
                </div>
                
                <RmDivider />
                <div class="p-2 flex items-center">
                    <RmTextButton @click="thisMonth">
                        Mês atual
                    </RmTextButton>
                    <div class="flex-1" />
                    <RmTextButton :class="{'opacity-50': !date1 || !date2}" 
                                  @click="() => { 
                                      if(!date1 || !date2) return;
                                      apply();
                                      toogle();
                                  }">
                        Aplicar
                    </RmTextButton>
                </div>
                
            </div>
        </template>
    </RmDropdown>

</template>