<script lang="ts" setup>
import { PropType, ref, nextTick } from 'vue';
import { DatePicker } from 'v-calendar';
import RmModal from './RmModal.vue';
import RmButton from './RmButton.vue';

const props = defineProps({
    canSelectFutureDates: {
        type: Boolean,
        default: false
    },
    isOpened: {
        type: Boolean,
        required: true
    },
    dates: {
        type: Object as PropType<{ start: Date, end: Date }>,
        required: true
    }
});

const emits = defineEmits(['update:isOpened', 'update:dates', 'on:filter']);

const datesTemp = ref({ start: props.dates.start, end: props.dates.end });

const cancel = () => {
    emits('update:isOpened', false);
    datesTemp.value.start = props.dates?.start;
    datesTemp.value.end = props.dates?.end;
};

const aplicarFiltro = () => {
    const newDate = { start: datesTemp.value.start, end: datesTemp.value.end };
    emits('update:dates', newDate);
    emits('update:isOpened', false);

    nextTick(() => emits('on:filter'));
};

</script>

<template>
    <RmModal :is-opened="isOpened" :no-padding="true">
        <template #default>
            <div class="px-4 pb-4 pt-2 w-full">
                <DatePicker v-model.range="datesTemp"
                            color="red"
                            :expanded="true"
                            :max-date="canSelectFutureDates ? null : new Date()" />
            </div>
        </template>
        <!-- Footer -->
        <template #footer>
            <RmButton type="modal-secondary" @click="cancel">
                Voltar
            </RmButton>
            <RmButton type="modal-primary" @click="aplicarFiltro">
                Filtrar
            </RmButton>
        </template>
    </RmModal>
</template>

<style>
.vc-container * {
    font-family: 'Open Sans';
}
.vc-container {
    border: none !important;
}
.vc-week {
    margin-bottom: 10px;
}
.vc-header {
    margin-bottom: 20px;
}
.vc-title {
    font-size: 1rem !important;
    font-weight: 600
}
.vc-title {
    font-weight: 700
}
</style>