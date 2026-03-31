<script lang="ts" setup>
import RmAdminPage from '../../components/RmAdminPage.vue';
import RmTab from '../../components/RmTab.vue';
import RmText from '../../components/RmText.vue';

</script>

<template>
    <RmAdminPage class="flex flex-col">

        <!-- Commissões -->
        <RmText type="display-medium" class="mb-2">
            Comissões
        </RmText>

        <!-- Tabs -->
        <div class="border-b border-gray-300 dark:border-gray-600 mb-4">
            <RmTab label="Últimas comissões"
                   :selected="$route.name == 'comissoes'" 
                   @on:pressed="$router.replace({ name: 'comissoes' })" />
        </div>

        <!-- Conteúdo -->
        <RouterView v-slot="{ Component }">
            <transition name="fade" mode="out-in">
                <component :is="Component" />
            </transition>
        </RouterView>

    </RmAdminPage>
</template>