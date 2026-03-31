<script lang="ts" setup>
defineProps({
    isOpened: {
        type: Boolean,
        default: false,
    },
    noPadding: {
        type: Boolean,
        default: false
    }
});

defineEmits<{
    (e: 'update:isOpened', v: boolean): void
}>();
</script>

<template>
    <Teleport to="body">
        <transition name="fade">
            <div v-if="isOpened"
                 class="fixed top-0 right-0 w-full h-screen z-50 bg-black bg-opacity-60 flex items-center justify-center" 
                 @click.self="$emit('update:isOpened', false)">
                <transition name="modalAppear"
                            appear>
                    <div class="bg-white dark:bg-gray-800 rounded-lg text-center w-80"
                         style="max-width: 90%">

                        <!-- Conteúdo -->
                        <div class="flex flex-col items-center rounded-lg shadow-lg"
                             :class="{'p-7': !noPadding}">
                            <slot />
                        </div>

                        <!-- Footer -->
                        <footer class="w-full flex py-2">
                            <slot name="footer" />
                        </footer>
                    </div>
                </transition>
            </div>
        </transition>
    </Teleport>
</template>