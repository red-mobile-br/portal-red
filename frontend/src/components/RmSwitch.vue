<script lang="ts" setup>
defineProps({
    label: {
        type: String,
        required: true
    },
    modelValue: {
        type: Boolean,
        required: true
    }
});

defineEmits(['update:modelValue']);
</script>

<template>
    <div>
        <label class="text-sm font-semibold block mb-1.5 dark:text-white">{{ label }}</label>

        <div class="flex items-center">
            <p class="text-sm font-semibold mr-3">
                Não
            </p>
            <div class="h-10 w-16 rounded-full transition-colors duration-150 shadow-inset bg-neutral-300 dark:bg-gray-500 flex items-center px-1 cursor-pointer"
                 :class="{'!bg-primary': modelValue}"
                 @click="$emit('update:modelValue', !modelValue)">
                <div class="h-8 w-8 bg-white dark:bg-gray-200 rounded-full shadow transform transition-transform duration-150"
                     :class="{'translate-x-6': modelValue}" />
            </div>
            <p class="text-sm font-semibold ml-3">
                Sim
            </p>
        </div>
    </div>
</template>