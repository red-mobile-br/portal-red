<script lang="ts" setup>
import { PropType, computed } from 'vue';
import isAfter from 'date-fns/isAfter';
import isBefore from 'date-fns/isBefore';
import isSameMonth from 'date-fns/isSameMonth';
import max from 'date-fns/max';
import min from 'date-fns/min';

import { meses } from '@/utils/meses';

const props = defineProps({
    date: {
        type: Object as PropType<Date>,
        required: true
    },
    selectedDate1: {
        type: Object as PropType<Date | null>,
    },
    selectedDate2: {
        type: Object as PropType<Date | null>,
    },
    hoverDate: {
        type: Object as PropType<Date | null>,
    }
});

const label = computed(() => meses[props.date.getMonth()].substring(0, 3));

const isSelected = computed(() => {
    if(props.selectedDate1 && isSameMonth(props.date, props.selectedDate1)) return true;
    if(props.selectedDate2 && isSameMonth(props.date, props.selectedDate2)) return true;
    return false;    
});

const isInRange = computed(() => {
    if(props.selectedDate1 && props.selectedDate2) {
        const minDate = min([props.selectedDate1, props.selectedDate2]);
        const maxDate = max([props.selectedDate1, props.selectedDate2]);
        return isAfter(props.date, minDate) && isBefore(props.date, maxDate);
    }

    if(props.selectedDate1 && !props.selectedDate2 && props.hoverDate) {
        const minDate = min([props.selectedDate1, props.hoverDate]);
        const maxDate = max([props.selectedDate1, props.hoverDate]);
        return isAfter(props.date, minDate) && isBefore(props.date, maxDate);
    }

    return false;
});

const backgroundClasses = computed(() => {
    if(isInRange.value) {
        return 'bg-primary-light w-full';
    }

    const isHovering = props.selectedDate1 && !props.selectedDate2 && props.hoverDate && !isSameMonth(props.hoverDate, props.selectedDate1);

    if(isHovering && isSameMonth(props.selectedDate1, props.date)) {
        const minDate = min([props.selectedDate1, props.hoverDate]);
        return isSameMonth(minDate, props.date)
            ? 'bg-primary-light w-1/2 right-0'
            : 'bg-primary-light w-1/2 left-0';
    }

    if(isHovering && isSameMonth(props.date, props.hoverDate)) {
        return isAfter(props.date, props.selectedDate1)
            ? 'bg-primary-light w-full rounded-r-full'
            : 'bg-primary-light w-full rounded-l-full';
    }

    if(
        props.selectedDate1 
        && props.selectedDate2
        && !isSameMonth(props.selectedDate1, props.selectedDate2)
        && (isSameMonth(props.selectedDate1, props.date) || isSameMonth(props.selectedDate2, props.date))
    ) {
        const minDate = min([props.selectedDate1, props.selectedDate2]);
        return isSameMonth(minDate, props.date)
            ? 'bg-primary-light w-1/2 right-0'
            : 'bg-primary-light w-1/2 left-0';
    }

    return '';
});


</script>

<template>
    <div class="cursor-pointer hover:bg-black/5 dark:hover:bg-white/5 relative">
        <div class="absolute top-0 bottom-0 z-0" 
             :class="backgroundClasses"
        />
        <div class="rounded-full  border-2 border-transparent text-sm w-full h-full flex items-center justify-center z-1 relative dark:text-white"
             :class="{'bg-primary text-white font-semibold border-white' : isSelected}">
            {{ label }}
        </div>
    </div>
</template>

