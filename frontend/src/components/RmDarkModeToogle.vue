<script lang="ts" setup>
import { ref, watch } from 'vue';
import { SunIcon, MoonIcon } from '../icons';

const darkModeActive = ref(document.documentElement.classList.contains('dark'));

watch(() => darkModeActive.value, (newValue) => {
    window.dispatchEvent(new Event('darkModeToogle'));
    localStorage.setItem('theme', newValue ? 'dark' : 'light');
    newValue
        ? document.documentElement.classList.add('dark')
        : document.documentElement.classList.remove('dark');
});
</script>

<template>
    <button class="flex items-center p-2 rounded-md hover:bg-gray-100 dark:hover:bg-gray-500"
            @click="darkModeActive = !darkModeActive">
            
        <SunIcon class="fill-neutral-900 dark:fill-white w-5"
                 :class="{'opacity-40': darkModeActive}" />

        <div class="h-5 bg-neutral-900 dark:bg-white w-px mx-2" />
        
        <MoonIcon class="fill-current text-neutral-900 dark:text-white w-5"
                  :class="{'opacity-40': !darkModeActive}" />
    </button>
</template>