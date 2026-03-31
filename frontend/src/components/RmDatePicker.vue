<script lang="ts" setup>
import { reactive } from 'vue';
import { RmButton, RmModal } from '.';
import { DatePicker } from 'v-calendar';

export interface SelectDateArgs {
    initialDate: Date;
    submitLabel?: string;
    canSelectFutureDates?: boolean;
}

interface DateModalState {
    isOpened: boolean;
    date: Date;
    submitLabel: string;
    canSelectFutureDates: boolean;
}

const state = reactive<DateModalState>({
    isOpened: false,
    date: new Date(),
    submitLabel: "Filtrar",
    canSelectFutureDates: false
});
        
let promiseResolve: ((date: Date | null) => void ) | null = null;

const selectDate = (args: SelectDateArgs) => {
    const { initialDate, submitLabel = "Filtrar", canSelectFutureDates = false } = args;

    state.date = initialDate;
    state.submitLabel = submitLabel;
    state.isOpened = true;
    state.canSelectFutureDates = canSelectFutureDates;
    return new Promise((resolve) => {
        promiseResolve = resolve;
    });
};

const finish = () => {
    state.isOpened = false;
    promiseResolve!(state.date as Date);
};

const cancel = () => {
    state.isOpened = false;
    promiseResolve!(null);
};

defineExpose({ selectDate });
</script>

<template>
    <RmModal :is-opened="state.isOpened" :no-padding="true">

        <template #default>
            <div class="px-4 pb-4 pt-2 w-full">
                <DatePicker v-model="state.date" 
                            color="red" 
                            is-expanded :max-date="state.canSelectFutureDates ? null : new Date()" />
            </div>
        </template>

        <!-- Footer -->
        <template #footer>
            <RmButton type="modal-secondary" @click="cancel">
                Voltar
            </RmButton>
            <RmButton type="modal-primary" @click="finish">
                {{ state.submitLabel }}
            </RmButton>
        </template>
    </RmModal>
</template>

<style>
.vc-container{
    @apply !bg-transparent;
}
.vc-title {
    @apply !text-gray-800 dark:!text-white;
}
.vc-day:not(.vc-red) {
    @apply !text-gray-800 dark:!text-white;
}
</style>