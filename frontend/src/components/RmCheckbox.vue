<script lang="ts" setup>
import CheckIcon from '@/icons/CheckIcon.vue';
defineProps({
    modelValue: {
        type: Boolean,
        required: true
    },
    label: {
        type: String
    }
});

defineEmits(['update:modelValue']);
</script>

<template>
    <div class="flex items-center">
        <div class="rounded w-5 h-5 border-2 border-gray-400 hover:bg-gray-100 cursor-pointer flex items-center justify-center transition duration-150 select-none"
             :class="{'!border-primary bg-primary hover:bg-primary': modelValue}"
             @click="$emit('update:modelValue', !modelValue)">
            <Transition name="modalAppear">
                <CheckIcon v-if="modelValue" class="w-4 fill-white" />

            </Transition>
        </div>

        <p class="font-semibold text-sm ml-3 cursor-pointer" @click="$emit('update:modelValue', !modelValue)">
            {{ label }}
        </p>
    </div>
</template>
