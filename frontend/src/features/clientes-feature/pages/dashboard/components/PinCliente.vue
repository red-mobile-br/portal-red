<script lang="ts" setup>
import { PropType } from 'vue';
import pin from '@/assets/img/pin.png';

defineProps({
    state: {
        type: String,
        required: true
    },
    selectedState: {
        type: String,
        required: true
    },
    scale: {
        type: Number,
        required: true
    },
    clientsAmount: {
        type: Number,
        required: true
    },
    stateName: {
        type: String
    },
    onSelect: {
        type: Function as PropType<(value: string) => void>,
        required: true
    }
});
</script>

<template>
    <div class="w-px h-px flex justify-center items-end relative" :class="{'z-50': state == selectedState}">

        <div class="bg-white rounded-xl shadow-lg px-4 pb-1 pt-2 text-center flex flex-col items-center mb-4 transition-opacity durantion-300 pointer-events-none absolute"
             :class="{'opacity-0': state != selectedState}"
             :style="{'bottom': `${(2.5 * scale)}rem`}">
            <p class="!text-neutral-900 font-semibold font-sans leading-4 whitespace-nowrap">
                {{ stateName }}
            </p>
            <p class="!text-neutral-900 font-bold font-sans text-lg leading-5">
                {{ clientsAmount }}
            </p>
            <p class="!text-neutral-900 font-sans">
                Clientes
            </p>
            <div class="w-4 h-4 rounded-b-sm bg-white -mb-3 transform rotate-45" />
        </div>

        <img :src="pin" class="max-w-none h-10 cursor-pointer"
             :style="{height: `${2.5 * scale}rem`}"
             @click="() => onSelect(state)">

    </div>
</template>

