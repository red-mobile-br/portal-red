<script lang="ts" setup>
import RmLoading from './RmLoading.vue';
defineProps({
    isLoading: {
        type: Boolean,
        required: true
    }
});
</script>

<template>
    <Teleport to="body">
        <transition name="fade">
            <div v-if="isLoading" class="fixed top-0 right-0 w-full h-screen bg-black bg-opacity-70 z-50 flex flex-col items-center justify-center">
                <p class="text-2xl font-bold text-white mb-4">
                    Carregando
                </p>
                <RmLoading />
            </div>
    
        </transition>

    </Teleport>
</template>