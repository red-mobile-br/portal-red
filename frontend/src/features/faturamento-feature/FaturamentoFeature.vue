<script lang="ts" setup>
import RmAdminPage from '../../components/RmAdminPage.vue';
import RmTab from '../../components/RmTab.vue';
import RmText from '../../components/RmText.vue';

</script>

<template>
    <RmAdminPage class="flex flex-col">

        <!-- Faturamento -->
        <RmText type="display-medium" class="mb-2">
            Faturamento
        </RmText>

        <!-- Tabs -->
        <div class="border-b border-gray-300 dark:border-gray-600 mb-4">
            <RmTab label="Últimas vendas" 
                   :selected="$route.name == 'faturamento'"
                   @on:pressed="$router.replace({ name: 'faturamento' })" />
        </div>

        <!-- Conteúdo -->
        <RouterView v-slot="{ Component }">
            <transition name="fade" mode="out-in">
                <component :is="Component" />
            </transition>
        </RouterView>

    </RmAdminPage>
</template>