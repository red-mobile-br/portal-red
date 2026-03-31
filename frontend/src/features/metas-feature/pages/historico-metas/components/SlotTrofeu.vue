<script lang="ts" setup>
import hexagonMask from '@/assets/img/hexagon-mask.svg';
</script>

<template>
    <div class=" w-20 h-20 relative bg-neutral-200 overflow-hidden flex items-center justify-center">

        <!-- Mascara -->
        <img :src="hexagonMask" class="absolute w-full h-full">
    </div>
</template>
