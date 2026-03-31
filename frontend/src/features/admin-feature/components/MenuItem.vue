<template>
    <div class="w-full px-2.5 mb-2">
        <div class="h-12 w-full rounded-md flex items-center lg:hover:bg-neutral-400 lg:dark:hover:bg-gray-700 cursor-pointer transition-all duration-300 relative px-3.5"
             :class="{
                 'bg-neutral-400 dark:bg-gray-700': selected,
             }">
            <component :is="(icons as any)[icon]"
                       class="w-6"
                       :class="selected ? 'fill-primary' : 'fill-gray-400'" />

            <div class="absolute left-9 transition-opacity duration-300"
                 :class="{'lg:opacity-0': contracted}">
                <p 
                    class="text-sm pl-2.5 text-neutral-900 dark:text-white opacity-50 whitespace-nowrap" 
                    :class="{'!opacity-100 font-bold': selected}">
                    {{ label }}
                </p>

            </div>
        </div>
    </div>
</template>

<script lang='ts' setup>
import * as icons from '../../../icons';

defineProps({
    icon: {
        type: String,
        required: true
    },
    label: {
        type: String,
        required: true
    },
    selected: {
        type: Boolean,
        default: false
    },
    contracted: {
        type: Boolean,
        required: true,
    }
});
</script>

<style>

</style>