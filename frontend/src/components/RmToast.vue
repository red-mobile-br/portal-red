<script lang="ts" setup>
import { ref } from 'vue';
import { Icons } from '../icons';
import * as icons from '../icons';
import { RmIconButton } from '.';

interface ItemToast {
    id: number;
    mensagem: string;
    icone: Icons;
    persistente: boolean;
    carregando: boolean;
}

const toasts = ref<ItemToast[]>([]);

const removerToast = (id: number) => {
    toasts.value = toasts.value.filter(el => el.id != id);
};

const exibir = (opcoes: { mensagem: string; icone?: Icons; persistente?: boolean; carregando?: boolean }) => {
    const id = Math.random();
    toasts.value.unshift({
        id,
        mensagem: opcoes.mensagem,
        icone: opcoes.icone || 'AlertIcon',
        persistente: opcoes.persistente || false,
        carregando: opcoes.carregando || false
    });

    if (!opcoes.persistente && !opcoes.carregando) {
        setTimeout(() => {
            removerToast(id);
        }, 5000);
    }

    return id;
};

const dispensar = (id: number) => {
    removerToast(id);
};

defineExpose({ exibir, dispensar });
</script>

<template>
    <div class="fixed toast z-50 flex flex-col items-end top-6 right-2 sm:right-7">
        <transition-group name="list">
            <div v-for="toast in toasts" :key="toast.id"
                 class="relative transition-all border border-gray-100 dark:border-gray-900 duration-300
                        rounded-lg bg-white dark:bg-gray-800 flex px-4 py-3 items-center shadow-2xl overflow-hidden mb-2">
                <!-- Spinner de carregamento -->
                <svg v-if="toast.carregando" class="w-6 h-6 animate-spin text-primary" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                <component v-else :is="icons[toast.icone]" class="w-6 fill-primary" />

                <p class="text-sm pl-4 pr-2 flex-1">
                    {{ toast.mensagem }}
                </p>
                <RmIconButton v-if="!toast.carregando" icon="TimesIcon" icon-class="w-3 fill-primary" @click="removerToast(toast.id)" />
                <div v-if="!toast.persistente && !toast.carregando" class="absolute bottom-0 bg-primary left-0 toast__progress" />
            </div>
        </transition-group>
    </div>
</template>

<style>
.toast p {
    max-width: 250px;
    min-width: 150px;
}
@keyframes progress {
    from { width: 0 }
    to { width: 100% }
}
.toast__progress {
    height: 3px;
    animation: progress 5s linear forwards;
}

/* Animação list */

.list-enter-from {
    opacity: 0;
    transform: translateX(-50px)
}
.list-enter-to {
    opacity: 1;
    transform: translateX(0)
}
.list-leave-to {
  opacity: 0;
}
</style>
