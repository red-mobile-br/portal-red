<script lang="ts" setup>
import RmAdminPage from '@/components/RmAdminPage.vue';
import RmTab from '@/components/RmTab.vue';
import RmText from '@/components/RmText.vue';

</script>

<template>
    <RmAdminPage class="flex flex-col">

        <!-- Pedidos -->
        <RmText type="display-medium" 
                class="mb-2">
            Pedidos
        </RmText>

        <!-- Tabs -->
        <div class="border-b border-gray-300 dark:border-gray-600 mb-4  overflow-x-auto whitespace-nowrap hidden-scroll">
            <RmTab label="Gerenciar pedidos"
                   :selected="$route.name == 'pedidos'"
                   @on:pressed="$router.replace({ name: 'pedidos' })" />
            <RmTab label="Consultar pedido"
                   :selected="$route.name == 'consultarPedido'"
                   @on:pressed="$router.replace({ name: 'consultarPedido' })" />
            <RmTab label="Novo pedido"
                   :selected="$route.name == 'novopedido'"
                   @on:pressed="$router.replace({ name: 'novopedido' })" />
            <RmTab v-if="$route.name == 'editarPedido'"
                   label="Edição de pedido"
                   :selected="$route.name == 'editarPedido'" />
        </div>

        <!-- Conteúdo -->
        <RouterView v-slot="{ Component }">
            <transition name="fade"
                        mode="out-in">
                <component :is="Component"
                           :key="$route.name" />
            </transition>
        </RouterView>

    </RmAdminPage>
</template>