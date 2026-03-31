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
            Orçamentos
        </RmText>

        <!-- Tabs -->
        <div class="border-b border-gray-300 dark:border-gray-600 mb-4  overflow-x-auto whitespace-nowrap hidden-scroll">
            <RmTab label="Gerenciar orçamentos"
                   :selected="$route.name == 'orcamentos'"
                   @on:pressed="$router.replace({ name: 'orcamentos' })" />

            <RmTab label="Consultar orçamento"
                   :selected="$route.name == 'consultarOrcamento'"
                   @on:pressed="$router.replace({ name: 'consultarOrcamento' })" />

            <RmTab label="Novo orçamento"
                   :selected="$route.name == 'novoOrcamento'"
                   @on:pressed="$router.replace({ name: 'novoOrcamento' })" />

            <RmTab v-if="$route.name == 'editarOrcamento'"
                   label="Edição de orçamento"
                   :selected="$route.name == 'editarOrcamento'" />
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