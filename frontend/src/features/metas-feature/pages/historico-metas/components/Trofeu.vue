<script lang="ts" setup>
import logo from '@/assets/img/logo.svg';
import thophyStar from '@/assets/img/trophy-star.svg';
</script>
<template>
    <div class="w-20 h-20 relative flex items-center justify-center">
        <div class="w-20 h-20 trophy p-0.5">
            <div class="trophy__content w-full h-full" />
        </div>

        <div class="w-4 h-3.5 rounded-sm bg-white absolute bottom-2 left-2"
             style="box-shadow: 0px 2px 2px rgba(0, 0, 0, 0.1);" />
        <div class="w-4 h-3.5 rounded-sm bg-white absolute bottom-2 right-2"
             style="box-shadow: 0px 2px 2px rgba(0, 0, 0, 0.1);" />
        <div class="h-4 w-11 rounded-sm bg-white absolute bottom-3 flex items-center justify-center"
             style="box-shadow: 0px 2px 2px rgba(0, 0, 0, 0.1);">
            <img :src="logo">
        </div>

        <img :src="thophyStar" class="absolute w-10" style="top: 1.1rem">
    </div>
</template>

<style>
.trophy {
    mask-image: url('~@/assets/img/hexagon-mask-fill.svg');
    mask-size: 100% 100%;
    mask-repeat: no-repeat;
    background: #953539;
}

.trophy__content {
    mask-image: url('~@/assets/img/hexagon-mask-fill.svg');
    mask-size: 100% 100%;
    mask-repeat: no-repeat;
    background: linear-gradient(180deg, #EA696F 0%, #CD3F45 100%);
}
</style>