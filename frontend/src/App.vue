<script setup lang="ts">
const theme = localStorage.getItem('theme');
if(theme && theme == 'dark') {
    document.documentElement.classList.add('dark');
}
</script>

<template>
    <div class="min-h-screen bg-neutral-200 dark:bg-gray-800 max-sm:overflow-x-hidden w-full">
        <RouterView v-slot="{Component}">
            <transition name="fade"
                        mode="out-in">
                <component :is="Component" />
            </transition>
        </RouterView>
    </div>
</template>