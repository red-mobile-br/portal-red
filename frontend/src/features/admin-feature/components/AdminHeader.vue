<script lang="ts" setup>
import RmIconButton from '../../../components/RmIconButton.vue';
import Logo from '../../../illustrations/Logo.vue';

defineEmits<{
    (e: 'menu:pressed'): void
}>();

</script>

<template>
    <div class="bg-white dark:bg-gray-900 dark:border-gray-800 h-15 border-b border-neutral-200 flex items-center pr-4 pl-1 lg:pl-8 fixed top-0 left-0 w-full shadow-sm z-20">

        <!-- Menu -->
        <RmIconButton icon="MenuIcon"
                      class="flex text-neutral-900 dark:text-white w-10 lg:hidden"
                      icon-class="fill-gray-900 dark:fill-neutral-200 w-6"
                      @click="$emit('menu:pressed')" />

        <!-- Logo -->
        <Logo class="h-7 mx-2 lg:h-8" />
        <div class="flex-1" />
        
        <!-- Conteúdo -->
        <slot />
    </div>
</template>