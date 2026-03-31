<script lang="ts" setup>
import { PrintIcon } from '@/icons';
import RmTextButton from './RmTextButton.vue';


const print = () => {
    window.print();
};
</script>

<template>
    <div class="bg-gray-200">
        <div class="bg-white h-12 border-b border-neutral-600 flex items-center shadow-sm fixed top-0 w-full pl-8 pr-2 print:hidden z-50">
            <p class="text-center font-semibold text-sm">
                Impressão de relatório
            </p>
            <div class="flex-1" />
            <RmTextButton @click="print">
                <div>Imprimir</div>
                <PrintIcon class="fill-primary w-4 ml-2" />
            </RmTextButton>
        </div>

        <div class="flex-1 min-h-screen py-20 print:py-0">
            <slot />
        </div>
    </div>
</template>
