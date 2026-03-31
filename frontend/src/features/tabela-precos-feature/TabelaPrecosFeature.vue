<script lang="ts" setup>
import RmAdminPage from '../../components/RmAdminPage.vue';
import RmTab from '../../components/RmTab.vue';
import RmText from '../../components/RmText.vue';

</script>

<template>
    <RmAdminPage class="flex flex-col">

        <!-- Tabela de preços -->
        <RmText type="display-medium" class="mb-2">
            Tabela de preços
        </RmText>

        <!-- Tabs -->
        <div class="border-b border-gray-300 dark:border-gray-600 mb-4">
            <RmTab label="Consultar tabela" 
                   :selected="$route.name == 'tabelaprecos'"
                   @on:pressed="$router.replace({ name: 'tabelaprecos' })" />
        </div>

        <!-- Conteúdo -->
        <RouterView v-slot="{ Component }">
            <transition name="fade" mode="out-in">
                <component :is="Component" />
            </transition>
        </RouterView>

    </RmAdminPage>
</template>