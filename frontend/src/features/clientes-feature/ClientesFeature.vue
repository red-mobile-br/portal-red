<script lang="ts" setup>
import RmAdminPage from '@/components/RmAdminPage.vue';
import RmTab from '@/components/RmTab.vue';
import RmText from '@/components/RmText.vue';

</script>

<template>
    <RmAdminPage class="flex flex-col">

        <!-- Clientes -->
        <RmText type="display-medium" class="mb-2">
            Clientes
        </RmText>

        <!-- Tabs -->
        <div class="border-b border-gray-300 dark:border-gray-600 mb-4">
            <RmTab label="Meus clientes" :selected="$route.name == 'clientes'" @on:pressed="$router.replace({ name: 'clientes' })" />
            <RmTab label="Consultar cliente" :selected="$route.name == 'consultar-cliente'" @on:pressed="$router.replace({ name: 'consultar-cliente' })" />
            <RmTab label="Cadastrar cliente" :selected="$route.name == 'cadastrar-cliente'" @on:pressed="$router.replace({ name: 'cadastrar-cliente' })" />
        </div>

        <!-- Conteúdo -->
        <RouterView v-slot="{ Component }">
            <transition name="fade" mode="out-in">
                <component :is="Component" />
            </transition>
        </RouterView>

    </RmAdminPage>
</template>