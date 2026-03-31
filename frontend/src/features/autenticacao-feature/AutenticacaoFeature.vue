<script lang="ts" setup>
import RmDarkModeToogle from '../../components/RmDarkModeToogle.vue';
import RmText from '../../components/RmText.vue';
import { bg } from '../../assets/img';
import { computed } from 'vue';

const year = computed(() => new Date().getFullYear());

</script>

<template>
    <div class="min-h-screen relative flex flex-col items-stretch">
        <!-- Container -->
        <div class="bg-gradient-to-br from-white to-neutral-100 overflow-hidden
                  dark:from-gray-800 dark:to-gray-700 flex items-center
                    justify-center flex-1 p-6 shadow-main rounded-bl-50px md:rounded-bl-100px relative">
            <RouterView v-slot="{ Component }">
                <transition name="fade"
                            mode="out-in">
                    <component :is="Component" />
                </transition>
            </RouterView>

            <img class="absolute top-0 right-0 hidden md:block opacity-60 h-[80%]"
                 :src="bg">
        </div>


        <!-- Dark Mode Toogle -->
        <RmDarkModeToogle class="absolute top-4 right-4" />

        <!-- Footer -->
        <footer class="h-12 flex flex-col md:flex-row justify-center items-center">
            <RmText type="label-medium"
                    class="md:mx-12">
                Copyright © {{ year }} Red Mobile - Todos os direitos reservados.
            </RmText>
            <RmText type="label-medium"
                    class="md:mx-12">
                Política de privacidade    |    Termos de uso
            </RmText>
        </footer>
    </div>
</template>