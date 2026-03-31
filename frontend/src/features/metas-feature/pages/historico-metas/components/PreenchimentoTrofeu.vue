<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';
import hexagonBackFill from '@/assets/img/hexagon-back-fill.svg';
import hexagonOutFill from '@/assets/img/hexagon-out-fill.svg';

const props = defineProps({
    percentage: {
        type: Number,
        required: true
    }
});
const translate = computed(() => {
    const percentage = Math.max(props.percentage, 20);
    return `transform: translateY(-${percentage}%)`;
});

const active = ref(false);

onMounted(() => {
    setTimeout(() => {
        active.value = true;
    }, 100);
});
</script>

<template>
    <div class=" w-20 h-20 relative bg-neutral-200 dark:bg-gray-800 overflow-hidden flex items-center justify-center">

        <!-- Ondas -->
        <div class="w-20 h-20 absolute top-full">
            <div class="relative w-20 h-20" :class="{'fill-active':active}" :style="translate">
                <img :src="hexagonBackFill" class="absolute top-0 h-8">
                <img :src="hexagonOutFill" class="absolute h-20 bottom-0">
            </div>
        </div>

        <p class="absolute font-bold ">
            {{ Math.floor(percentage) }}%
        </p>

        <!-- Mascara -->
        <svg class="w-full h-full absolute top-0 left-0" viewBox="0 0 90 90" fill="none" xmlns="http://www.w3.org/2000/svg">
            <g clip-path="url(#clip0_1505_342)">
                <g filter="url(#filter0_d_1505_342)">
                    <path class="fill-white dark:fill-gray-700" fill-rule="evenodd" clip-rule="evenodd" d="M90 0H0V90H90V0ZM49 5.88452C46.5248 4.45546 43.4752 4.45545 41 5.88452L13.125 21.9782C10.6498 23.4072 9.12498 26.0482 9.12498 28.9064V61.0936C9.12498 63.9518 10.6498 66.5928 13.125 68.0218L41 84.1155C43.4752 85.5445 46.5248 85.5445 49 84.1155L76.875 68.0218C79.3502 66.5928 80.875 63.9518 80.875 61.0936V28.9064C80.875 26.0482 79.3502 23.4072 76.875 21.9782L49 5.88452Z" />
                </g>
            </g>
            <defs>
                <filter id="filter0_d_1505_342" x="-4" y="0" width="98" height="98" filterUnits="userSpaceOnUse" color-interpolation-filtros="sRGB">
                    <feFlood flood-opacity="0" result="BackgroundImageFix" />
                    <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha" />
                    <feOffset dy="4" />
                    <feGaussianBlur stdDeviation="2" />
                    <feComposite in2="hardAlpha" operator="out" />
                    <feColorMatrix type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0.25 0" />
                    <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_1505_342" />
                    <feBlend mode="normal" in="SourceGraphic" in2="effect1_dropShadow_1505_342" result="shape" />
                </filter>
                <clipPath id="clip0_1505_342">
                    <rect width="90" height="90" class="fill-white dark:fill-gray-700" />
                </clipPath>
            </defs>
        </svg>


    </div>
</template>

<style>
@keyframes out-fill {
    0% { transform: translateX(0); }
    100% {transform: translateX(-50%);}
}

.fill-active > img:first-child {
    animation: out-fill 2s linear infinite;
}

.fill-active > img:last-child {
    animation: out-fill 4s linear infinite;
}

</style>