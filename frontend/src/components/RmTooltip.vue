<script lang="ts" setup>
defineProps({
    text: {
        type: String,
        required: true
    }
});
</script>

<template>
    <div class="relative group">
        <slot />

        <div class="absolute right-full flex items-center opacity-0 translate-x-10 transition-all duration-300 group-hover:opacity-100 group-hover:translate-x-0 pointer-events-none"
             style="top: calc(50% - 1rem)">

            <div class="px-4 h-8 text-sm whitespace-nowrap font-bold bg-white dark:bg-gray-800 rounded-md shadow-sm flex items-center"
                 style="box-shadow: 0px 2px 30px rgba(0, 0, 0, .3);">
                {{ text }}
            </div>
            <div class="w-3 h-3 -ml-1.5 bg-white dark:bg-gray-800 rotate-45 rounded-tr-sm" />

        </div>
    </div>
</template>